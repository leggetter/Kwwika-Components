using System;
using System.Messaging;
using Kwwika.Common.Logging;

namespace Kwwika.QueueComponents
{
    public class MessageQueueReader
    {
        MessageQueueReadProcessor _processor;
        ILoggingService _logging;
        MessageQueue _readQueue;
        IMessageConsumer _consumer;
        string _queueName;
        Type _messageType;

        bool _started = false;

        public MessageQueueReader(string queueName, IMessageConsumer messageConsumer, Type messageType, ILoggingService logging)
        {
            _queueName = queueName;
            _messageType = messageType;
            _logging = logging;
            _consumer = messageConsumer;
        }

        public void Start()
        {
            if (_started == false)
            {
                _logging.Info("Starting MessageQueueReader");
                _started = true;
                _readQueue = GetQueue();
                _processor = new MessageQueueReadProcessor(_readQueue, _consumer, _messageType, _logging);
                _processor.Start();
            }
        }

        public void Stop()
        {
            _logging.Info("Stopping MessageQueueReader");
            _processor.Stop();
            _started = false;
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
