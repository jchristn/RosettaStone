using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FindClosestString;
using GetSomeInput;
using RosettaStone.Core;
using RosettaStone.Core.Services;
using SyslogLogging;
using Timestamps;
using Watson.ORM;
using WatsonWebserver;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace RosettaStone.Server
{
    public static class Program
    {
        #region Public-Members

        #endregion

        #region Private-Members

        private static string _Header = "[RosettaStone] ";
        private static SerializationHelper _Serializer = new SerializationHelper();
        private static string _SettingsFile = "./rosettastone.json";
        private static Settings _Settings = new Settings();
        private static bool _CreateDefaultRecords = false;
        private static LoggingModule _Logging = null;
        private static WatsonORM _ORM = null;
        private static CodecMetadataService _Codecs = null;
        private static VendorMetadataService _Vendors = null;
        private static WatsonWebserver.Server _Server = null;

        #endregion

        #region Entrypoint

        public static void Main(string[] args)
        {
            Welcome();
            InitializeSettings(args);
            InitializeGlobals();

            if (_Settings.EnableConsole)
            {
                RunConsoleWorker();
            }
            else
            {
                EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
                bool waitHandleSignal = false;
                do
                {
                    waitHandleSignal = waitHandle.WaitOne(1000);
                }
                while (!waitHandleSignal);
            }
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        private static void Welcome()
        {
            Console.WriteLine(
                Environment.NewLine +
                Constants.Logo +
                Constants.ProductName + 
                Environment.NewLine);
        }

        private static void InitializeSettings(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.StartsWith("--config="))
                    {
                        _SettingsFile = arg.Substring(9);
                    }
                    else if (arg.Equals("--setup"))
                    {
                        _CreateDefaultRecords = true;
                    }
                }
            }

            if (!File.Exists(_SettingsFile))
            {
                Console.WriteLine("Settings file '" + _SettingsFile + "' does not exist, creating with defaults");
                File.WriteAllBytes(_SettingsFile, Encoding.UTF8.GetBytes(_Serializer.SerializeJson(_Settings, true)));
            }
            else
            {
                _Settings = _Serializer.DeserializeJson<Settings>(File.ReadAllText(_SettingsFile));
                Console.WriteLine("Loaded settings from file '" + _SettingsFile + "'");
            }
        }

        private static void InitializeGlobals()
        {
            #region Logging

            Console.WriteLine("Initializing logging to " + _Settings.Logging.SyslogServerIp + ":" + _Settings.Logging.SyslogServerPort);

            _Logging = new LoggingModule(
                _Settings.Logging.SyslogServerIp,
                _Settings.Logging.SyslogServerPort,
                _Settings.EnableConsole);

            if (!String.IsNullOrEmpty(_Settings.Logging.LogDirectory)
                && !Directory.Exists(_Settings.Logging.LogDirectory))
            {
                Directory.CreateDirectory(_Settings.Logging.LogDirectory);
                _Settings.Logging.LogFilename = _Settings.Logging.LogDirectory + _Settings.Logging.LogFilename;
            }

            if (!String.IsNullOrEmpty(_Settings.Logging.LogFilename))
            {
                _Logging.Settings.FileLogging = FileLoggingMode.FileWithDate;
                _Logging.Settings.LogFilename = _Settings.Logging.LogFilename;
            }

            #endregion

            #region ORM

            Console.WriteLine("Initializing database");
            _ORM = new WatsonORM(_Settings.Database);

            _ORM.InitializeDatabase();
            _ORM.InitializeTables(new List<Type>
            {
                typeof(CodecMetadata),
                typeof(VendorMetadata)
            });

            #endregion

            #region Services

            _Codecs = new CodecMetadataService(_Logging, _ORM);
            _Vendors = new VendorMetadataService(_Logging, _ORM);

            #endregion

            #region Default-Records

            if (_CreateDefaultRecords)
            {
                VendorMetadata vendor1 = new VendorMetadata
                {
                    Key = "ACTGACTGACTGACTGACTGACTGACTGAC",
                    Name = "Vendor 1",
                    ContactInformation = "100 S Main St, San Jose, CA 95128",
                };

                VendorMetadata vendor2 = new VendorMetadata
                {
                    Key = "GTCAGTCAGTCAGTCAGTCAGTCAGTCAAC",
                    Name = "Vendor 2",
                    ContactInformation = "200 S Vine St, Campbell, CA 95008",
                };

                VendorMetadata vendor3 = new VendorMetadata
                {
                    Key = "CATGCATGCATGCATGCATGCATGCATGAC",
                    Name = "Vendor 3",
                    ContactInformation = "300 N 1st St, San Jose, CA 95128",
                };

                vendor1 = _Vendors.Add(vendor1);
                Console.WriteLine("Creating vendor " + vendor1.Key + " " + vendor1.Name);
                vendor2 = _Vendors.Add(vendor2);
                Console.WriteLine("Creating vendor " + vendor2.Key + " " + vendor2.Name);
                vendor3 = _Vendors.Add(vendor3);
                Console.WriteLine("Creating vendor " + vendor3.Key + " " + vendor3.Name);

                CodecMetadata codec1 = new CodecMetadata
                {
                    VendorGUID = vendor1.GUID,
                    Key = "CAGTCAGTCAGTCAGTCAGTCAGTCAGTAC",
                    Name = "My CODEC",
                    Version = "v1.0.0",
                    Uri = "https://codec1.com"
                };

                CodecMetadata codec2 = new CodecMetadata
                {
                    VendorGUID = vendor2.GUID,
                    Key = "TCAGTCAGTCAGTCAGTCAGTCAGTCAGAC",
                    Name = "My CODEC",
                    Version = "v2.0.0",
                    Uri = "https://codec1.com"
                };

                CodecMetadata codec3 = new CodecMetadata
                {
                    VendorGUID = vendor3.GUID,
                    Key = "TAGCTAGCTAGCTAGCTAGCTAGCTAGCAC",
                    Name = "My CODEC",
                    Version = "v3.0.0",
                    Uri = "https://codec1.com"
                };

                codec1 = _Codecs.Add(codec1);
                Console.WriteLine("Creating CODEC " + codec1.Key + " " + codec1.Name);
                codec2 = _Codecs.Add(codec2);
                Console.WriteLine("Creating CODEC " + codec2.Key + " " + codec2.Name);
                codec3 = _Codecs.Add(codec3);
                Console.WriteLine("Creating CODEC " + codec3.Key + " " + codec3.Name);
            }

            #endregion

            #region Webserver

            _Server = new WatsonWebserver.Server(
                _Settings.Webserver.DnsHostname,
                _Settings.Webserver.Port,
                _Settings.Webserver.Ssl,
                DefaultRoute);

            _Server.Start();

            Console.WriteLine("Webserver started on " +
                (_Settings.Webserver.Ssl ? "https://" : "http://") +
                _Settings.Webserver.DnsHostname + ":" +
                _Settings.Webserver.Port);

            #endregion
        }

        private static void RunConsoleWorker()
        {
            bool runForever = true;

            while (runForever)
            {
                string userInput = Inputty.GetString("Command [?/help]:", null, false);

                switch (userInput)
                {
                    case "q":
                        runForever = false;
                        break;
                    case "c":
                    case "cls":
                        Console.Clear();
                        break;
                    case "?":
                        Console.WriteLine("");
                        Console.WriteLine("Available commands:");
                        Console.WriteLine("q         quit, exit the application");
                        Console.WriteLine("cls       clear the screen");
                        Console.WriteLine("?         help, this menu");
                        Console.WriteLine("");
                        break;
                }
            }
        }

        private static async Task DefaultRoute(HttpContext ctx)
        {
            Timestamp ts = new Timestamp();

            #region Variables-and-Query-Values

            CodecMetadata codec = null;
            VendorMetadata vendor = null;
            List<CodecMetadata> codecs = null;
            List<VendorMetadata> vendors = null;
            
            Dictionary<string, object> ret = null;

            int maxResults = 10;

            string maxResultsStr = ctx.Request.Query.Elements.Get("results");

            if (!String.IsNullOrEmpty(maxResultsStr))
                maxResults = Convert.ToInt32(maxResultsStr);

            #endregion

            ctx.Response.ContentType = Constants.JsonContentType;

            try
            {
                switch (ctx.Request.Method)
                {
                    case HttpMethod.GET:
                        if (ctx.Request.Url.Elements.Length == 0)
                        {
                            ctx.Response.StatusCode = 200;
                            ctx.Response.ContentType = Constants.HtmlContentType;
                            await ctx.Response.Send(Constants.RootHtml);
                            return;
                        }
                        else if (ctx.Request.Url.Elements.Length == 1)
                        {
                            if (ctx.Request.Url.Elements[0].ToLower().Equals("favicon.ico"))
                            {
                                ctx.Response.StatusCode = 200;
                                await ctx.Response.Send();
                                return;
                            }
                        }
                        else if (ctx.Request.Url.Elements.Length == 2)
                        {
                            if (ctx.Request.Url.Elements[0] == "v1.0")
                            {
                                if (ctx.Request.Url.Elements[1] == "vendor")
                                {
                                    // get all vendors
                                    vendors = _Vendors.All();
                                    ctx.Response.StatusCode = 200;
                                    await ctx.Response.Send(_Serializer.SerializeJson(vendors, true));
                                    return;
                                }
                                else if (ctx.Request.Url.Elements[1] == "codec")
                                {
                                    // get all CODECs
                                    codecs = _Codecs.All();
                                    ctx.Response.StatusCode = 200;
                                    await ctx.Response.Send(_Serializer.SerializeJson(codecs, true));
                                    return;
                                }
                            }
                        }
                        else if (ctx.Request.Url.Elements.Length == 3)
                        {
                            if (ctx.Request.Url.Elements[0] == "v1.0")
                            {
                                if (ctx.Request.Url.Elements[1] == "vendor")
                                {
                                    // get vendor by key
                                    vendor = _Vendors.GetByKey(ctx.Request.Url.Elements[2]);
                                    if (vendor == null)
                                    {
                                        ret = new Dictionary<string, object>
                                        {
                                            { "Message", Constants.NotFoundError }
                                        };

                                        ctx.Response.StatusCode = 404;
                                        await ctx.Response.Send(_Serializer.SerializeJson(ret, true));
                                        return;
                                    }
                                    else
                                    {
                                        ctx.Response.StatusCode = 200;
                                        await ctx.Response.Send(_Serializer.SerializeJson(vendor, true));
                                        return;
                                    }
                                }
                                else if (ctx.Request.Url.Elements[1] == "codec")
                                {
                                    // get CODEC by key
                                    codec = _Codecs.GetByKey(ctx.Request.Url.Elements[2]);
                                    if (codec == null)
                                    {
                                        ret = new Dictionary<string, object>
                                        {
                                            { "Message", Constants.NotFoundError }
                                        };

                                        ctx.Response.StatusCode = 404;
                                        await ctx.Response.Send(_Serializer.SerializeJson(ret, true));
                                        return;
                                    }
                                    else
                                    {
                                        ctx.Response.StatusCode = 200;
                                        await ctx.Response.Send(_Serializer.SerializeJson(codec, true));
                                        return;
                                    }
                                }
                            }
                        }
                        else if (ctx.Request.Url.Elements.Length == 4)
                        {
                            if (ctx.Request.Url.Elements[0] == "v1.0")
                            {
                                if (ctx.Request.Url.Elements[1] == "vendor")
                                {
                                    if (ctx.Request.Url.Elements[2] == "match")
                                    {
                                        // get closest vendor match by key
                                        vendor = _Vendors.FindClosestMatch(ctx.Request.Url.Elements[3]);
                                        ctx.Response.StatusCode = 200;
                                        await ctx.Response.Send(_Serializer.SerializeJson(vendor, true));
                                        return;
                                    }
                                    else if (ctx.Request.Url.Elements[2] == "matches")
                                    {
                                        // get closest vendor matches by key
                                        vendors = _Vendors.FindClosestMatches(ctx.Request.Url.Elements[3], maxResults);
                                        ctx.Response.StatusCode = 200;
                                        await ctx.Response.Send(_Serializer.SerializeJson(vendors, true));
                                        return;
                                    }
                                }
                                else if (ctx.Request.Url.Elements[1] == "codec")
                                {
                                    if (ctx.Request.Url.Elements[2] == "match")
                                    {
                                        // get closest CODEC match by key
                                        codec = _Codecs.FindClosestMatch(ctx.Request.Url.Elements[3]);
                                        ctx.Response.StatusCode = 200;
                                        await ctx.Response.Send(_Serializer.SerializeJson(codec, true));
                                        return;
                                    }
                                    else if (ctx.Request.Url.Elements[2] == "matches")
                                    {
                                        // get closest CODEC matches by key
                                        codecs = _Codecs.FindClosestMatches(ctx.Request.Url.Elements[3], maxResults);
                                        ctx.Response.StatusCode = 200;
                                        await ctx.Response.Send(_Serializer.SerializeJson(codecs, true));
                                        return;
                                    }
                                }
                                else if (ctx.Request.Url.Elements[1] == "full")
                                {
                                    if (ctx.Request.Url.Elements[2] == "match")
                                    {
                                        string key = ctx.Request.Url.Elements[3];
                                        if (key.Length < 36)
                                        {
                                            _Logging.Warn(_Header + "supplied key is 35 characters or less");
                                            ret = new Dictionary<string, object>
                                            {
                                                { "Message", Constants.BadRequestError },
                                                { "Context", "Supplied key must be greater than 35 characters." }
                                            };

                                            ctx.Response.StatusCode = 404;
                                            await ctx.Response.Send(_Serializer.SerializeJson(ret, true));
                                            return;
                                        }

                                        // left 35, right 35
                                        string left = key.Substring(0, 35);
                                        string right = key.Substring((key.Length - 35), 35);

                                        ResultSet resultSet = new ResultSet
                                        {
                                            Key = key,
                                            Left = left,
                                            Right = right,
                                            Vendor = _Vendors.FindClosestMatch(left),
                                            Codec = _Codecs.FindClosestMatch(right)
                                        };

                                        ctx.Response.StatusCode = 200;
                                        await ctx.Response.Send(_Serializer.SerializeJson(resultSet, true));
                                        return;
                                    }
                                    else if (ctx.Request.Url.Elements[2] == "matches")
                                    {
                                        string key = ctx.Request.Url.Elements[3];
                                        if (key.Length < 36)
                                        {
                                            _Logging.Warn(_Header + "supplied key is 35 characters or less");
                                            ret = new Dictionary<string, object>
                                            {
                                                { "Message", Constants.BadRequestError },
                                                { "Context", "Supplied key must be greater than 35 characters." }
                                            };

                                            ctx.Response.StatusCode = 404;
                                            await ctx.Response.Send(_Serializer.SerializeJson(ret, true));
                                            return;
                                        }

                                        // left 35, right 35
                                        string left = key.Substring(0, 35);
                                        string right = key.Substring((key.Length - 35), 35);

                                        ResultSet resultSet = new ResultSet
                                        {
                                            Key = key,
                                            Left = left,
                                            Right = right,
                                            Vendors = _Vendors.FindClosestMatches(left, maxResults),
                                            Codecs = _Codecs.FindClosestMatches(right, maxResults)
                                        };

                                        ctx.Response.StatusCode = 200;
                                        await ctx.Response.Send(_Serializer.SerializeJson(resultSet, true));
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                }

                ret = new Dictionary<string, object>
                {
                    { "Message", Constants.BadRequestError }
                };

                ctx.Response.StatusCode = 400;
                await ctx.Response.Send(_Serializer.SerializeJson(ret, true));
            }
            catch (Exception e)
            {
                _Logging.Exception(e);

                ret = new Dictionary<string, object>
                {
                    { "Message", Constants.InternalServerError },
                    { "Exception", e }
                };

                ctx.Response.StatusCode = 500;
                await ctx.Response.Send(_Serializer.SerializeJson(ret, true));
            }
            finally
            {
                ts.End = DateTime.UtcNow;

                _Logging.Debug(
                    _Header +
                    ctx.Request.Method.ToString() + " " +
                    ctx.Request.Url.RawWithoutQuery + " " +
                    ctx.Response.StatusCode + " " +
                    ts.TotalMs + "ms");
            }
        }

        #endregion
    }
}