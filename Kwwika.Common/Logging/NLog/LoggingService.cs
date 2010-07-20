using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Kwwika.Common.Logging.NLog
{
    public class LoggingService: ILoggingService
    {
        private Logger _logger;
        public LoggingService(string loggerName)
        {
            _logger = LogManager.GetLogger(loggerName);
        }

        #region ILoggerService Members

        public void Trace(object obj)
        {
            _logger.Trace(obj);
        }

        public void Debug(object obj)
        {
            _logger.Debug(obj);
        }

        public void Info(object obj)
        {
            _logger.Info(obj);
        }

        public void Warn(object obj)
        {
            _logger.Warn(obj);
        }

        public void Error(object obj)
        {
            _logger.Error(obj);
        }

        public void Fatal(object obj)
        {
            _logger.Fatal(obj);
        }

        #endregion
    }
}
