using Core.Messages;
using NLog;
using RLCCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.RemoteOperations
{
    public class MessageReceiveConfirmationService : IDisposable
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<int, MessageReceiveConfimationAgent> confirmationAgents = new Dictionary<int, MessageReceiveConfimationAgent>();
        private readonly Action<int, RLCMessage> resendAction;

        public MessageReceiveConfirmationService(IEnumerable<RemoteClient> clients, Action<int, RLCMessage> resendAction, int msResendDelay = 25, int msResendTimeout = 2000)
        {
            if(clients is null) {
                throw new ArgumentNullException(nameof(clients));
            }

            this.resendAction = resendAction ?? throw new ArgumentNullException(nameof(resendAction));

            foreach(var client in clients) {
                var agent = new MessageReceiveConfimationAgent(GetResendActionForClient(client.Number), msResendDelay, msResendTimeout);
                confirmationAgents.Add(client.Number, agent);
            }
        }

        private Action<RLCMessage> GetResendActionForClient(int clientId)
        {
            return (message) => resendAction.Invoke(clientId, message);
        }

        public void AddMessageToConfirmForAll(RLCMessage message)
        {
            if(message == null) {
                return;
            }

            foreach(var agent in confirmationAgents.Values) {
                agent.AddToConfirm(message);
            }
            LogJobsCount();
        }

        public void AddMessageToConfirm(int clientId, RLCMessage message)
        {
            if(message == null || clientId < 1) {
                return;
            }

            if(confirmationAgents.TryGetValue(clientId, out MessageReceiveConfimationAgent agent)) {
                agent.AddToConfirm(message);
            }
            LogJobsCount();
        }

        public void Confirm(RLCMessage response)
        {
            if(response == null || response.MessageId <= 0) {
                return;
            }

            if(confirmationAgents.TryGetValue(response.ClientNumber, out MessageReceiveConfimationAgent agent)) {
                agent.Confirm(response);
            }
        }

        private void LogJobsCount()
        {
            logger.Debug($"Jobs count: {confirmationAgents.Values.Sum(x => x.JobsCount)}");
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue) {
                if(disposing) {
                    foreach(var agent in confirmationAgents.Values) {
                        agent.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
