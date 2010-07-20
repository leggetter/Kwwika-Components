using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kwwika.Logging;
using Kwwika.Common.Logging;
using System.Threading;

namespace Kwwika.Common.Logging
{
    public class KwwikaLoggerWrapper: ILogger
    {
        ILoggingService _logService;
        public KwwikaLoggerWrapper(ILoggingService logService)
        {
            _logService = logService;
        }

        #region ILogger Members

        public void Log(LogLevels level, string[] categories, string message)
        {
            message = "(" + Thread.CurrentThread.ManagedThreadId + ") " + message + " : " + string.Join(",", categories);
            switch (level)
            {
                case LogLevels.Info:
                case LogLevels.Config:
                    _logService.Info(message);
                    break;
                case LogLevels.Critical:
                    _logService.Fatal(message);
                    break;
                case LogLevels.Debug:
                case LogLevels.Finer:
                case LogLevels.Finest:
                    _logService.Trace(message);
                    break;
                case LogLevels.Notify:
                case LogLevels.Warning:
                    _logService.Warn(message);
                    break;
                case LogLevels.Error:
                default:
                    _logService.Error(message);
                    break;
            }
        }

        public void Log(LogLevels level, string[] categories, string message, params object[] messageParameters)
        {
            Log(level, categories, string.Format(message, messageParameters));
        }        

        public void Log(LogLevels level, string category, string message, params object[] messageParameters)
        {
            Log(level, new string[] { category }, string.Format(message, messageParameters));
        }

        public void Log(LogLevels level, string category, string message)
        {
            Log(level, new string[] { category }, message);
        }

        #endregion
    }
}
