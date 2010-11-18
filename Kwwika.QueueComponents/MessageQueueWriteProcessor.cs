using System;
using System.Messaging;
using Kwwika.Common.Logging;

namespace Kwwika.QueueComponents
{
    internal class MessageQueueWriteProcessor
    {
        const int ALLOWED_FAILURE_RATE = 20;
        
        private System.Messaging.MessageQueue _writeQueue;
        private Common.Logging.ILoggingService _logger;

        public MessageQueueWriteProcessor(MessageQueue _writeQueue, Type messageType, ILoggingService _logging)
        {
            this._writeQueue = _writeQueue;
            this._writeQueue.Formatter = new XmlMessageFormatter(new Type[] { messageType });
            this._logger = _logging;
        }

        internal bool Write(object message)
        {
            bool messageWritten = false;

            _logger.Info("writing to message queue");
             MessageQueueTransaction myTransaction = new MessageQueueTransaction();
            _logger.Trace("message transaction begin");
            
            myTransaction.Begin();
            try
            {
                _logger.Trace("beginning message processing");
                _writeQueue.Send(message, myTransaction);

                myTransaction.Commit();
                _logger.Trace("message transaction commit");

                messageWritten = true;
            }
            catch (Exception ex)
            {
                myTransaction.Abort();
                _logger.Trace("message transaction abort");

                string error =
                    string.Format("Exception sending Message: {0}.\nException: {1}",
                    message, ex);
                _logger.Error(error);
            }
            return messageWritten;
        }
    }
}
