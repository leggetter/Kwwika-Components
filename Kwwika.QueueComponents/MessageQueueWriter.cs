using System;
using System.Messaging;
using Kwwika.Common.Logging;

namespace Kwwika.QueueComponents
{
    public class MessageQueueWriter
    {
        MessageQueueWriteProcessor _processor;
        ILoggingService _logging;
        MessageQueue _writeQueue;

        string _queueName;

        public MessageQueueWriter(string queueName, Type messageType, ILoggingService logging)
        {
            _queueName = queueName;
            _logging = logging;
            _writeQueue = GetQueue();
            _processor = new MessageQueueWriteProcessor(_writeQueue, messageType, _logging);
        }

        public void Write(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _processor.Write(message);
        }

        private MessageQueue GetQueue()
        {
            _logging.Info("Getting message queue");

            MessageQueue queue = null;
            if (MessageQueue.Exists(_queueName))
            {
                _logging.Info("MessageQueue already exists. Reusing the existing queue.");
                queue = new MessageQueue(_queueName);
            }
            else
            {
                _logging.Info("MessageQueue does not exists. Creating new queue.");
                queue = MessageQueue.Create(_queueName, true);
            }
            return queue;
        }
    }
}
