using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kwwika.Common.Logging
{
    public class ConsoleLogger: ILoggingService
    {
        private void Write(string level, object obj)
        {
            Console.WriteLine("{0}: {1}", level, obj);
        }

        #region ILoggingService Members

        public void Trace(object obj)
        {
            Write("Trace", obj);
        }

        public void Debug(object obj)
        {
            Write("Debug", obj);
        }

        public void Info(object obj)
        {
            Write("Info", obj);
        }

        public void Warn(object obj)
        {
            Write("Warn", obj);
        }

        public void Error(object obj)
        {
            Write("Error", obj);
        }

        public void Fatal(object obj)
        {
            Write("Fatal", obj);
        }

        #endregion
    }
}
