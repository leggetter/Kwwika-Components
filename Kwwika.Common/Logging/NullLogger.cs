using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kwwika.Common.Logging
{
    public class NullLogger: ILoggingService
    {
        private void Write(string level, object obj)
        {
            Console.WriteLine("{0}: {1}", level, obj);
        }

        #region ILoggingService Members

        public void Trace(object obj)
        {
        }

        public void Debug(object obj)
        {
        }

        public void Info(object obj)
        {
        }

        public void Warn(object obj)
        {
        }

        public void Error(object obj)
        {
        }

        public void Fatal(object obj)
        {
        }

        #endregion
    }
}
