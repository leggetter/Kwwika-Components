using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kwwika.TestsHelpers
{
    public class SubscriptionListenerHelper: ISubscriptionListener, ICommandListener
    {
        public ISubscription LastTopicUpdateForSubscription;
        public CommandErrorType LastTopicUpdateError;
        public Dictionary<string, string> LastTopicUpdate;
        public bool LastTopicUpdateWasImage;
        public bool LastPublishWasSuccessful;
        private IConnection _connection;

        public SubscriptionListenerHelper(IConnection connection)
        {
            _connection = connection;
        }

        #region ISubscriptionListener Members

        public void TopicError(ISubscription sub, CommandErrorType error)
        {
            LastTopicUpdateForSubscription = sub;
            LastTopicUpdateError = error;

            lock (this)
            {
                Monitor.Pulse(this);
            }
        }

        public void TopicUpdated(ISubscription sub, Dictionary<string, string> values, bool isImage)
        {
            LastTopicUpdateForSubscription = sub;
            LastTopicUpdate = values;
            LastTopicUpdateWasImage = isImage;

            lock (this)
            {
                Monitor.Pulse(this);
            }
        }

        #endregion

        #region ICommandListener Members

        public void CommandError(string topic, CommandErrorType code)
        {
            lock (this)
            {
                //LastTopicUpdateForSubscription = sub;
                LastPublishWasSuccessful = false;
                LastTopicUpdateError = code;

                Monitor.Pulse(this);
            }
        }

        public void CommandSuccess(string topic)
        {
            lock (this)
            {
                LastPublishWasSuccessful = true;

                Monitor.Pulse(this);
            }
        }

        #endregion

        public bool SubscribeAndWaitForUpdate(string topicName, int secondsToWait)
        {
            bool receivedUpdate = false;

            _connection.Subscribe(topicName, this);

            lock (this)
            {
                Monitor.Wait(this, secondsToWait * 1000);

                if (LastTopicUpdate != null)
                {
                    receivedUpdate = true;
                }
            }

            return receivedUpdate;
        }

        public bool SubscribeAndWaitForError(string topicName, CommandErrorType commandErrorType, int secondsToWait)
        {
            bool receivedError = false;

            _connection.Subscribe(topicName, this);

            lock (this)
            {
                Monitor.Wait(this, secondsToWait * 1000);

                if (LastTopicUpdateError == commandErrorType)
                {
                    receivedError = true;
                }
            }

            return receivedError;
        }

        public bool PublishAndWaitForError(string topicName, Dictionary<string, string> values, CommandErrorType commandErrorType, int secondsToWait)
        {
            bool receivedError = false;

            _connection.Publish(topicName, values, this);

            lock (this)
            {
                Monitor.Wait(this, secondsToWait * 1000);

                if (LastTopicUpdateError == commandErrorType)
                {
                    receivedError = true;
                }
            }

            return receivedError;
        }

        public bool PublishAndWaitForPublishSuccess(string topicName, Dictionary<string, string> values, int secondsToWait)
        {
            bool receivedPublishSuccess = false;

            _connection.Publish(topicName, values, this);

            lock (this)
            {
                Monitor.Wait(this, secondsToWait * 1000);

                receivedPublishSuccess = LastPublishWasSuccessful;
            }

            return receivedPublishSuccess;
        }

        public bool PublishAndWaitForUpdate(string topicName, Dictionary<string, string> values, int secondsToWait)
        {
            bool receivedMatchingUpdate = false;

            _connection.Subscribe(topicName, this);

            _connection.Publish(topicName, values, this);

            DateTime startTime = DateTime.Now;

            lock (this)
            {
                while (LastTopicUpdate == null && secondsToWait > 0)
                {
                    Monitor.Wait(this, secondsToWait * 1000);

                    secondsToWait = secondsToWait - (DateTime.Now - startTime).Seconds;
                }
                if (LastTopicUpdate != null)
                {
                    receivedMatchingUpdate = CheckDictionaryContentsMatch(LastTopicUpdate, values);
                }
            }

            return receivedMatchingUpdate;
        }

        public static bool CheckDictionaryContentsMatch(Dictionary<string, string> dictionary1, Dictionary<string, string> dictionary2)
        {
            bool allExpectedValuesReceived = true;
            foreach (KeyValuePair<string, string> value in dictionary1)
            {
                if (!dictionary2.Keys.Contains(value.Key) || dictionary2[value.Key] != value.Value)
                {
                    allExpectedValuesReceived = false;
                    break;
                }
            }
            return allExpectedValuesReceived;
        }
    }
}
