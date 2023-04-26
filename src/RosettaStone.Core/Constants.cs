﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosettaStone.Core
{
    public static class Constants
    {
        public static string Logo =
            @"                    _   _              _                    " + Environment.NewLine +
            @" _ __ ___  ___  ___| |_| |_ __ _   ___| |_ ___  _ __   ___  " + Environment.NewLine +
            @"| '__/ _ \/ __|/ _ \ __| __/ _` | / __| __/ _ \| '_ \ / _ \ " + Environment.NewLine +
            @"| | | (_) \__ \  __/ |_| || (_| | \__ \ || (_) | | | |  __/ " + Environment.NewLine +
            @"|_|  \___/|___/\___|\__|\__\__,_| |___/\__\___/|_| |_|\___| " + Environment.NewLine +
            Environment.NewLine;

        public static string ProductName = "Rosetta Stone Server";
        public static string InternalServerError = "An internal server error was encountered.";
        public static string BadRequestError = "Your request was invalid.  Please refer to the API documentation.";
        public static string NotFoundError = "The requested object was not found.";
    }
}
