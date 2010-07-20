using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kwwika.Common.Logging
{
    public interface ILoggingService
    {
        void Trace(object obj);
        void Debug(object obj);
        void Info(object obj);
        void Warn(object obj);
        void Error(object obj);
        void Fatal(object obj);
    }
}
