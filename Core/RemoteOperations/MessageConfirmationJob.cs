using Core.Messages;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.RemoteOperations
{
    public class MessageConfirmationJob : IDisposable
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private CancellationTokenSource cts;
        private readonly Action<RLCMessage> resendAction;
        private readonly int msResendDelay;
        private bool isStarted;

        public RLCMessage Message { get; }

        public event EventHandler<int> OnExpired;

        public MessageConfirmationJob(RLCMessage message, Action<RLCMessage> resendAction, int msResendDelay, int msResendTimeout)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            this.resendAction = resendAction ?? throw new ArgumentNullException(nameof(resendAction));
            this.msResendDelay = msResendDelay;
            cts = new CancellationTokenSource(msResendTimeout);
        }

        public int MessageId => Message.MessageId;

        public void Start()
        {
            if(isStarted) {
                return;
            }
            StartResending();
            isStarted = true;
        }

        private void StartResending()
        {
            var delayTask = Task.Delay(msResendDelay, cts.Token);
            delayTask.ContinueWith((prevTask) => Expire(), TaskContinuationOptions.OnlyOnCanceled);
            delayTask.ContinueWith((prevTask) => ResendAndRestart(), cts.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        private void ResendAndRestart()
        {
            var resendTask = Task.Run(() => Resend(), cts.Token);
            resendTask.ContinueWith((prevTask) => Expire(), TaskContinuationOptions.OnlyOnCanceled);
            resendTask.ContinueWith((prevTask) => StartResending(), cts.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        private void Resend()
        {
            try {
                resendAction.Invoke(Message);
            }
            catch(Exception ex) {
                cts.Cancel();
            }
        }

        private void Expire()
        {
            logger.Debug($"Job with MessageId: {MessageId} was expired");
            OnExpired?.Invoke(this, MessageId);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue) {
                if(disposing) {
                    cts.Cancel();
                    cts.Dispose();
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
