using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kwwika.Logging;

namespace Kwwika.TestsHelpers
{
    public class LoggerHelper: ILogger
    {
        private string[] _ignoreMessagesContaining;
        public LoggerHelper(string[] ignoreMessagesContaining)
        {
            _ignoreMessagesContaining = ignoreMessagesContaining;
        }

        #region ILogger Members

        public void Log(LogLevels level, string[] categories, string message, params object[] messageParameters)
        {
            Log(level, categories, string.Format(message, messageParameters));
        }

        public void Log(LogLevels level, string[] categories, string message)
        {
            foreach (string msgPart in _ignoreMessagesContaining)
            {
                if (message.Contains(msgPart))
                {
                    return;
                }
            }
            Console.WriteLine(level + ": " + String.Join(",", categories) + ": " + message);
        }

        public void Log(LogLevels level, string category, string message, params object[] messageParameters)
        {
            Log(level, new string[]{category}, string.Format(message, messageParameters));
        }

        public void Log(LogLevels level, string category, string message)
        {
            Log(level, new string[] { category }, message);
        }

        #endregion
    }
}
