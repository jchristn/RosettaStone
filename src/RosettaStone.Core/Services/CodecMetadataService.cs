using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionTree;
using FindClosestString;
using SyslogLogging;
using Watson.ORM;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace RosettaStone.Core.Services
{
    public class CodecMetadataService
    {
        #region Public-Members

        #endregion

        #region Private-Members

        private LoggingModule _Logging = null;
        private WatsonORM _ORM = null;

        #endregion

        #region Constructors-and-Factories

        public CodecMetadataService(LoggingModule logging, WatsonORM orm)
        {
            _Logging = logging ?? throw new ArgumentNullException(nameof(logging));
            _ORM = orm ?? throw new ArgumentNullException(nameof(orm));
        }

        #endregion

        #region Public-Methods

        public List<CodecMetadata> All()
        {
            Expr expr = new Expr(
                _ORM.GetColumnName<CodecMetadata>(nameof(CodecMetadata.Id)),
                OperatorEnum.GreaterThan,
                0
                );

            return _ORM.SelectMany<CodecMetadata>(expr);
        }

        public List<CodecMetadata> AllByVendor(string vendorGuid)
        {
            if (String.IsNullOrEmpty(vendorGuid)) throw new ArgumentNullException(nameof(vendorGuid));

            vendorGuid = vendorGuid.ToUpper();

            Expr expr = new Expr(
                _ORM.GetColumnName<CodecMetadata>(nameof(CodecMetadata.VendorGUID)),
                OperatorEnum.Equals,
                vendorGuid
                );

            return _ORM.SelectMany<CodecMetadata>(expr);
        }

        public CodecMetadata GetByKey(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            key = key.ToUpper();

            Expr expr = new Expr(
                _ORM.GetColumnName<CodecMetadata>(nameof(CodecMetadata.Key)),
                OperatorEnum.Equals,
                key
                );

            return _ORM.SelectFirst<CodecMetadata>(expr);
        }

        public bool ExistsByKey(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            key = key.ToUpper();

            Expr expr = new Expr(
                _ORM.GetColumnName<CodecMetadata>(nameof(CodecMetadata.Key)),
                OperatorEnum.Equals,
                key
                );

            return _ORM.Exists<CodecMetadata>(expr);
        }

        public CodecMetadata GetByGuid(string guid)
        {
            if (String.IsNullOrEmpty(guid)) throw new ArgumentNullException(nameof(guid));

            guid = guid.ToUpper();

            Expr expr = new Expr(
                _ORM.GetColumnName<CodecMetadata>(nameof(CodecMetadata.GUID)),
                OperatorEnum.Equals,
                guid
                );

            return _ORM.SelectFirst<CodecMetadata>(expr);
        }

        public bool ExistsByGuid(string guid)
        {
            if (String.IsNullOrEmpty(guid)) throw new ArgumentNullException(nameof(guid));

            guid = guid.ToUpper();

            Expr expr = new Expr(
                _ORM.GetColumnName<CodecMetadata>(nameof(CodecMetadata.GUID)),
                OperatorEnum.Equals,
                guid
                );

            return _ORM.Exists<CodecMetadata>(expr);
        }

        public List<CodecMetadata> Search(Expr expr, int startIndex, int maxResults)
        {
            if (expr == null) throw new ArgumentNullException(nameof(expr));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (maxResults > 1000) throw new ArgumentOutOfRangeException(nameof(maxResults));

            return _ORM.SelectMany<CodecMetadata>(startIndex, maxResults, expr);
        }

        public CodecMetadata Add(CodecMetadata cm)
        {
            if (cm == null) throw new ArgumentNullException(nameof(cm));

            cm.Key = cm.Key.ToUpper();
            cm.GUID = cm.GUID.ToUpper();
            cm.VendorGUID = cm.VendorGUID.ToUpper();

            if (ExistsByGuid(cm.GUID)) throw new ArgumentException("Object with GUID '" + cm.GUID + "' already exists.");
            if (ExistsByKey(cm.Key)) throw new ArgumentException("Object with key '" + cm.Key + "' already exists.");

            return _ORM.Insert<CodecMetadata>(cm);
        }

        public CodecMetadata Update(CodecMetadata cm)
        {
            if (cm == null) throw new ArgumentNullException(nameof(cm));

            cm.Key = cm.Key.ToUpper();
            cm.GUID = cm.GUID.ToUpper();
            cm.VendorGUID = cm.VendorGUID.ToUpper();

            return _ORM.Update<CodecMetadata>(cm);
        }

        public void DeleteByGuid(string guid)
        {
            if (String.IsNullOrEmpty(guid)) throw new ArgumentNullException(nameof(guid));

            guid = guid.ToUpper();

            Expr expr = new Expr(
                _ORM.GetColumnName<CodecMetadata>(nameof(CodecMetadata.GUID)),
                OperatorEnum.Equals,
                guid
                );

            _ORM.DeleteMany<CodecMetadata>(expr);
        }

        public void DeleteByKey(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            key = key.ToUpper();

            Expr expr = new Expr(
                _ORM.GetColumnName<CodecMetadata>(nameof(CodecMetadata.Key)),
                OperatorEnum.Equals,
                key
                );

            _ORM.DeleteMany<CodecMetadata>(expr);
        }

        public void DeleteByVendorGuid(string vendorGuid)
        {
            if (String.IsNullOrEmpty(vendorGuid)) throw new ArgumentNullException(nameof(vendorGuid));

            vendorGuid = vendorGuid.ToUpper();

            Expr expr = new Expr(
                _ORM.GetColumnName<CodecMetadata>(nameof(CodecMetadata.VendorGUID)),
                OperatorEnum.Equals,
                vendorGuid
                );

            _ORM.DeleteMany<CodecMetadata>(expr);
        }

        public CodecMetadata FindClosestMatch(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            key = key.ToUpper();

            List<CodecMetadata> all = All();
            List<string> keys = all.Select(x => x.Key).ToList();

            (string, int) result = ClosestString.UsingLevenshtein(key, keys);

            return GetByKey(result.Item1);
        }

        public List<CodecMetadata> FindClosestMatches(string key, int maxResults = 10)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (maxResults < 1 || maxResults > 10) throw new ArgumentOutOfRangeException(nameof(maxResults));

            key = key.ToUpper();

            List<CodecMetadata> all = All();
            List<string> keys = all.Select(x => x.Key).ToList();

            List<(string, int)> result = ClosestStrings.UsingLevenshtein(key, keys, maxResults);
            List<CodecMetadata> ret = new List<CodecMetadata>();

            foreach ((string, int) item in result)
            {
                CodecMetadata codec = GetByKey(item.Item1);
                codec.EditDistance = item.Item2;
                ret.Add(codec);
            }

            return ret;
        }

        #endregion

        #region Private-Methods

        #endregion
    }
}
