using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kwwika.TestsHelpers
{
    public class ConnectionListenerHelper: IConnectionListener
    {
        public ConnectionStatus Status = ConnectionStatus.Disconnected;

        #region IConnectionListener Members

        public void ConnectionStatusUpdated(ConnectionStatus status)
        {
            lock (this)
            {
                Status = status;
                Monitor.Pulse(this);
            }
        }

        #endregion

        public bool WaitForLoggedInStatus(int secondsToWait)
        {
            return WaitForStatus(new ConnectionStatus[] { ConnectionStatus.LoggedIn, ConnectionStatus.Reconnected }, secondsToWait);
        }

        public bool WaitForDisconnectedStatus(int secondsToWait)
        {
            return WaitForStatus(new ConnectionStatus[] { ConnectionStatus.Disconnected }, secondsToWait);
        }

        public bool WaitForLoggedFailedStatus(int secondsToWait)
        {
            return WaitForStatus(new ConnectionStatus[] { ConnectionStatus.LoginFailed }, secondsToWait);
        }

        public bool WaitForStatus(ConnectionStatus[] statuses, int secondsToWait)
        {
            bool statusFound = false;
            int secondsElapsed = 0;
            DateTime startTime = DateTime.Now;

            lock (this)
            {
                while (statusFound == false && secondsElapsed < secondsToWait)
                {
                    Monitor.Wait(this, secondsToWait * 1000);

                    if (statuses.Contains(Status))
                    {
                        statusFound = true;
                        break;
                    }

                    secondsElapsed = (DateTime.Now - startTime).Seconds;
                }
            }

            return statusFound;
        }
    }
}
