using System;
using System.Linq;
using RLCCore.Transport;
using Service;

namespace RLCCore.RemoteOperations
{
    public class RemoteClientsOperator : NotifyPropertyBase, IClientsOperatorStateInformer
    {
        private readonly IControlUnit controlUnit;
        private readonly ICommunicationService communicationService;
        private readonly IGlobalCommandsContextProvider globalContextProvider;

        public event EventHandler<OperatorStateEventArgs> StateChanged;

        private OperatorStates state;
        public OperatorStates State {
            get => state;
            private set {
                if(SetField(ref state, value, () => State)) {
                    StateChanged?.Invoke(this, new OperatorStateEventArgs(state));
                }
            }
        }
        
        public RemoteClientsOperator(IControlUnit controlUnit, ICommunicationService communicationService, IGlobalCommandsContextProvider networkSettingProvider)
        {
            this.controlUnit = controlUnit;
            this.communicationService = communicationService;
            this.globalContextProvider = networkSettingProvider;

            communicationService.OnReceiveMessage += CommunicationService_OnReceiveMessage;

            State = OperatorStates.Stop;
        }

        private void CommunicationService_OnReceiveMessage(object sender, IRemoteClientMessage e)
        {
            switch(e.MessageType) {
                case ClientMessages.SendPlateNumber:
                    UpdateClientState(e.ClientNumber);
                    break;
                case ClientMessages.RequestServerIP:
                    SendServerIP();
                    break;
                default:
                    break;
            }
        }

        #region private methods

        private void UpdateClientState(int clientNumber)
        {
            var client = controlUnit.Clients.FirstOrDefault(x => x.Number == clientNumber);
            client.UpdateStatus(true);
        }

        private void SendServerIP()
        {
            var sendServerAddressContext = globalContextProvider.GetCommandContext(GlobalCommands.SendServerAddress);
            communicationService.SendGlobalCommand(GlobalCommands.SendServerAddress, sendServerAddressContext);
        } 

        #endregion

        public void Start()
        {
            var state = GlobalCommands.Play;
            var playContext = globalContextProvider.GetCommandContext(state);
            communicationService.SendGlobalCommand(state, playContext);
            State = OperatorStates.Play;
        }

        public void Stop()
        {
            var state = GlobalCommands.Stop;
            var stopContext = globalContextProvider.GetCommandContext(state);
            communicationService.SendGlobalCommand(state, stopContext);
            State = OperatorStates.Stop;
        }

        public void Pause()
        {
            var state = GlobalCommands.Pause;
            var pauseContext = globalContextProvider.GetCommandContext(state);
            communicationService.SendGlobalCommand(state, pauseContext);
            State = OperatorStates.Pause;
        }

        public void StartFrom()
        {
            var state = GlobalCommands.PlayFrom;
            var playFromContext = globalContextProvider.GetCommandContext(state);
            communicationService.SendGlobalCommand(state, playFromContext);
            State = OperatorStates.Play;
        }
    }
}
