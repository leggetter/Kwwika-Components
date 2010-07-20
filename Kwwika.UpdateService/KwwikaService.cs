using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Kwwika;
using System.Messaging;
using Kwwika.Common.Logging;
using Kwwika.Common.Logging.NLog;
using Kwwika.QueueComponents;

namespace KwwikaUpdateService
{
    public partial class KwwikaService : ServiceBase
    {
        const string KWWIKA_PUBLISH_QUEUE = @".\Private$\KwwikaPublishQueue";
        const string KWWIKA_API_KEY = "p4eyeB9u";
        const string KWWIKA_DOMAIN = "www.optasports.com";

        ILoggingService _logging;
        MessageQueueReader _reader;

        public KwwikaService()
        {
            InitializeComponent();
            _logging = new LoggingService("KwwikaService");
        }

        protected override void OnStart(string[] args)
        {
            _reader = new MessageQueueReader(KWWIKA_API_KEY, KWWIKA_DOMAIN, KWWIKA_PUBLISH_QUEUE, _logging);
            _reader.Start();
        }

        protected override void OnStop()
        {
            _reader.Stop();
        }
    }
}
