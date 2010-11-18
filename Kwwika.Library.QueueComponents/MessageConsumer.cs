using System;
using System.Threading;
using Kwwika.Common.Logging;
using Kwwika.QueueComponents;

namespace Kwwika.Library.QueueComponents
{
    public class MessageConsumer: IMessageConsumer, ICommandListener, IConnectionListener
    {
        const int ALLOWED_FAILURE_RATE = 20;
        private IConnection _conn;
        private ILoggingService _logger;

        ConnectionStatus _status = ConnectionStatus.Disconnected;
        
        public MessageConsumer(string kwwikaApiKey, string kwwikaDomain, string kwwikaServiceQueueName, ILoggingService logger)
        {
            this._conn = Kwwika.Service.Connect(kwwikaApiKey, kwwikaDomain);
            this._conn.AddConnectionListener(this);
            this._conn.Logger = new KwwikaLoggerWrapper(logger);
            _logger = logger;           
        }

        #region ICommandListener Members

        public void CommandError(string topic, CommandErrorType code)
        {
            _logger.Error(string.Format("Error publishing to {0}. Reason: {1}", topic, code));
        }

        public void CommandSuccess(string topic)
        {
            _logger.Trace(string.Format("Successfully publishing to {0}.", topic));
        }

        #endregion

        #region IConnectionListener Members

        public void ConnectionStatusUpdated(ConnectionStatus status)
        {
            _status = status;
        }

        #endregion

        #region IMessageConsumer Members

        public bool ProcessMessage(object msg)
        {
            bool published = false;
            if (_status == ConnectionStatus.LoggedIn || _status == ConnectionStatus.Reconnected)
            {                
                PublishMessage message = (PublishMessage)msg;
                if (string.IsNullOrEmpty(message.TopicName))
                {
                    throw new ArgumentException("message.TopicName cannot be null or empty");
                }
                _conn.Publish(message.TopicName, message.Values, this);
                published = true;
            }
            else
            {
                _logger.Warn("Kwwika connection is not established. Aborting message handling and sleeping");
                Thread.Sleep(5000);
            }
            return published;
        }

        #endregion
    }
}

