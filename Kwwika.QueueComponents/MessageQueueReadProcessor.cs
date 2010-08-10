using System;
using System.Threading;
using System.Messaging;
using Kwwika.Common.Logging;

namespace Kwwika.QueueComponents
{
    public class MessageQueueReadProcessor: ICommandListener, IConnectionListener
    {
        const int ALLOWED_FAILURE_RATE = 20;
        private MessageQueue _readQueue;
        private IConnection _conn;
        private ILoggingService _logger;

        ConnectionStatus _status = ConnectionStatus.Disconnected;
        
        private int _failureCount = 0;
        bool _fatalFailure = false;

        Thread _queueReader;
        bool _reading = false;
        
        public MessageQueueReadProcessor(MessageQueue readQueue, IConnection conn, Type messageType, ILoggingService logger)
        {
            _readQueue = readQueue;
            _readQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(PublishMessage) });

            this._conn = conn;
            this._conn.AddConnectionListener(this);
            this._conn.Logger = new KwwikaLoggerWrapper(logger);
            _logger = logger;           
        }

        public void Stop()
        {
            _logger.Info("stopping MessageQueueReadProcessor");
            _reading = false;

            bool terminated = _queueReader.Join(10000);
            if (terminated == false)
            {
                _logger.Error("Queue reader thread could not be joined. Aborting thread.");
                _queueReader.Abort();
            }
        }

        public void Start()
        {
            if (_reading == false)
            {
                _logger.Info("starting MessageQueueReadProcessor");
                if (_fatalFailure == false)
                {
                    _reading = true;
                    if (_queueReader != null &&
                        _queueReader.IsAlive)
                    {
                        _logger.Error("Queue reader thread is still working. Using existing reader thread");

                    }
                    else
                    {
                        _queueReader = new Thread(ReadQueue);
                        _queueReader.IsBackground = true;
                        _queueReader.Start();
                    }
                }
                else
                {
                    // don't log. Fatal failure email has already been sent.
                }
            }
            else
            {
                _logger.Error("Call to start when MessageQueueReadProcessor is still reading. Call will be ignored.");
            }
        }

        private void ReadQueue()
        {
            _logger.Info("queue reader thread started");
            while (_reading && _fatalFailure == false)
            {
                MessageQueueTransaction myTransaction = new MessageQueueTransaction();
                _logger.Trace("message transaction begin");
                myTransaction.Begin();

                Message msg = null;
                try
                {
                    msg = _readQueue.Receive(myTransaction);
                    _logger.Info("message received");

                    if (_status == ConnectionStatus.LoggedIn || _status == ConnectionStatus.Reconnected)
                    {
                        _logger.Trace("beginning message processing");

                        ProcessMessage(msg);

                        _logger.Trace("finished message processing");
                        myTransaction.Commit();

                        _logger.Trace("message transaction commit");

                        _logger.Trace("resetting failure count");
                        _failureCount = 0;
                    }
                    else
                    {
                        _logger.Warn("Kwwika connection is not established. Aborting message handling and sleeping");
                        myTransaction.Abort();
                        Thread.Sleep(5000);
                    }
                }
                catch (InvalidOperationException ioe)
                {
                    // badly formed message. Just log and discard.
                    myTransaction.Commit();
                    _logger.Trace("message transaction committed follow invalid formatted message");

                    string error =
                        string.Format("Badly formed message. Discarding: {0}.\nException: {1}",
                        msg, ioe);
                    _logger.Error(error);
                }
                catch (ThreadAbortException tae)
                {
                    if (_reading == false)
                    {
                        // caused by a stop()
                    }
                    else
                    {
                        _logger.Error("Unexpected thread abort exception: " + tae.ToString());
                    }
                }
                catch (ArgumentException ae)
                {
                    // Message to large. Throw away.
                    myTransaction.Commit();
                    _logger.Trace("message transaction committed follow excessively large message");

                    string error =
                        string.Format("Excessively large message. Discarding: {0}.\nException: {1}",
                        msg, ae);
                    _logger.Error(error);
                }
                catch (Exception ex)
                {
                    myTransaction.Abort();
                    _logger.Trace("message transaction abort");

                    _failureCount++;

                    _logger.Info("failure count increased to" + _failureCount);

                    string error =
                        string.Format("Exception processing Message: {0}.\nFailure Count: {1}.\nException: {2}",
                        msg, _failureCount, ex);
                    _logger.Error(error);

                    if (_failureCount > ALLOWED_FAILURE_RATE)
                    {
                        _fatalFailure = true;
                        string failureMsg = string.Format("Failure count {0} exceeded maximum failures {1}. Queue will no longer be read.",
                            _failureCount, ALLOWED_FAILURE_RATE);

                        _logger.Fatal(failureMsg);
                    }
                }
            }
        }

        private void ProcessMessage(Message msg)
        {
            PublishMessage publish = (PublishMessage)msg.Body;
            _conn.Publish(publish.TopicName, publish.Values, this);            
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
    }
}

