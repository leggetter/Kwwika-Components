using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Messaging;
using Kwwika.Common.Logging;
using Kwwika;

namespace Kwwika.QueueComponents
{
    public class MessageQueueReader
    {
        MessageQueueReadProcessor _processor;
        ILoggingService _logging;
        IConnection _conn;
        MessageQueue _readQueue;

        string _kwwikaApiKey;
        string _kwwikaDomain;
        string _kwwikaServiceQueueName;

        bool _started = false;

        public MessageQueueReader(string kwwikaApiKey, string kwwikaDomain, string kwwikaServiceQueueName, ILoggingService logging)
        {
            _kwwikaApiKey = kwwikaApiKey;
            _kwwikaDomain = kwwikaDomain;
            _kwwikaServiceQueueName = kwwikaServiceQueueName;
            _logging = logging;            
        }

        public void Start()
        {
            if (_started == false)
            {
                _started = true;
                _readQueue = GetQueue();
                _conn = Service.Connect(_kwwikaApiKey, _kwwikaDomain);
                _processor = new MessageQueueReadProcessor(_readQueue, _conn, typeof(PublishMessage), _logging);
                _processor.Start();
            }
        }

        public void Stop()
        {
            _logging.Info("stopping MessageQueueReader");
            _processor.Stop();
            _started = false;
        }

        private MessageQueue GetQueue()
        {
            _logging.Info("Getting message queue");

            MessageQueue queue = null;
            if (MessageQueue.Exists(_kwwikaServiceQueueName))
            {
                _logging.Info("MessageQueue already exists. Reusing the existing queue.");
                queue = new MessageQueue(_kwwikaServiceQueueName);
            }
            else
            {
                _logging.Info("MessageQueue does not exists. Creating new queue.");
                queue = MessageQueue.Create(_kwwikaServiceQueueName, true);
            }
            return queue;
        } 
    }
}
