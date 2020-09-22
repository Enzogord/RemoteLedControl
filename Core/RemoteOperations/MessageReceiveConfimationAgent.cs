using Core.Messages;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Core.RemoteOperations
{
    public sealed class MessageReceiveConfimationAgent : IDisposable
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Action<RLCMessage> resendOperation;
        private readonly int msResendDelay;
        private readonly int msResendTimeout;
        private readonly ConcurrentDictionary<int, MessageConfirmationJob> confirmationJobs = new ConcurrentDictionary<int, MessageConfirmationJob>();

        public MessageReceiveConfimationAgent(Action<RLCMessage> resendOperation, int msResendDelay, int msResendTimeout)
        {
            this.resendOperation = resendOperation ?? throw new ArgumentNullException(nameof(resendOperation));
            this.msResendDelay = msResendDelay;
            this.msResendTimeout = msResendTimeout;
        }

        public int JobsCount => confirmationJobs.Count;

        public void AddToConfirm(RLCMessage message)
        {
            if(!MustConfirm(message)) {
                return;
            }
            try {
                AddJob(message);
            }
            catch(Exception ex) {
                logger.Error(ex, "Произошла ошибка при добавлении сообщения на подтверждение получения клиентом");
            }
        }

        private bool MustConfirm(RLCMessage message)
        {
            return message != null && message.MessageType == MessageType.State;
        }

        private void AddJob(RLCMessage message)
        {
            if(confirmationJobs.ContainsKey(message.MessageId)) {
                return;
            }
            DeleteMessageConfirmation(message.MessageId);
            MessageConfirmationJob job = new MessageConfirmationJob(message, resendOperation, msResendDelay, msResendTimeout);
            if(confirmationJobs.TryAdd(message.MessageId, job)) {
                job.OnExpired += Job_OnExpired;
                job.Start();
            }
        }

        private void Job_OnExpired(object sender, int e)
        {
            Task.Run(() => DeleteMessageConfirmation(e));
        }

        public void Confirm(RLCMessage response)
        {
            if(response == null) {
                return;
            }
            DeleteMessageConfirmation(response.MessageId);
        }

        private void DeleteMessageConfirmation(int messageId)
        {
            confirmationJobs.TryRemove(messageId, out MessageConfirmationJob job);
            try {
                job?.Dispose();
            }
            catch(ObjectDisposedException) {
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        public void Dispose(bool disposing)
        {
            if(!disposedValue) {
                if(disposing) {
                    foreach(var job in confirmationJobs) {
                        job.Value?.Dispose();
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
