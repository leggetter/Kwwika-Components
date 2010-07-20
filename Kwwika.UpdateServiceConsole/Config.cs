using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KwwikaUpdateServiceConsole
{
    internal static class Config
    {
        // Permission for /KWWIKA/SANDBOX
        public static readonly string PublishQueue = @".\Private$\KwwikaPublishQueue";
        public static readonly string KwwikaApiKey = "e4e80610-9406-11df-981c-0800200c9a66";
        public static readonly string KwwikaDomain = "kwwika.com";
    }
}
