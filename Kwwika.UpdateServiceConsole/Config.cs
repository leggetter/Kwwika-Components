using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace KwwikaUpdateServiceConsole
{
    internal static class Config
    {
        // Permission for /KWWIKA/SANDBOX
        public static readonly string PublishQueue = ConfigurationManager.AppSettings["PublishQueue"] ?? @".\Private$\KwwikaPublishQueue";
        public static readonly string KwwikaApiKey = ConfigurationManager.AppSettings["KwwikaApiKey"] ?? "e4e80610-9406-11df-981c-0800200c9a66";
        public static readonly string KwwikaDomain = ConfigurationManager.AppSettings["KwwikaDomain"] ?? "kwwika.com";
    }
}
