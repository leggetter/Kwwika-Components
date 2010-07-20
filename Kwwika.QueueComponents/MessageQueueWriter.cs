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
    public class MessageQueueWriter
    {
        MessageQueueWriteProcessor _processor;
        ILoggingService _logging;
        MessageQueue _writeQueue;

        string _kwwikaServiceQueueName;

        public MessageQueueWriter(string kwwikaServiceQueueName, ILoggingService logging)
        {
            _kwwikaServiceQueueName = kwwikaServiceQueueName;
            _logging = logging;
            _writeQueue = GetQueue();
            _processor = new MessageQueueWriteProcessor(_writeQueue, _logging, typeof(PublishMessage));
        }

        public void Write(object message)
        {
            _processor.Write(message);
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
