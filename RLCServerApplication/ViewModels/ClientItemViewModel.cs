using Core.RemoteOperations;
using RLCCore.Domain;
using RLCServerApplication.Infrastructure;
using System;

namespace RLCServerApplication.ViewModels
{
    public class ClientItemViewModel : ViewModelBase
    {
        private readonly IClientConnectionProvider connectionProvider;
        private IClientConnection connection;

        public ClientItemViewModel(RemoteClient client, IClientConnectionProvider connectionProvider)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
            this.connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            connectionProvider.ConnectionsUpdated += (s, e) => UpdateConnection();
            ConfigureBindings();
        }

        private void UpdateConnection()
        {
            if(!connectionProvider.TryGetClientConnection(Client.Number, out connection)) {
                return;
            }

            CreateNotificationBinding()
                .AddProperty(nameof(Connected))
                .SetNotifier(connection)
                .BindToProperty(x => x.Connected)
                .End();
        }

        private void ConfigureBindings()
        {
            
            CreateNotificationBinding()
                .AddProperty(nameof(Name))
                .SetNotifier(Client)
                .BindToProperty(x => x.Name)
                .End();
            CreateNotificationBinding()
                .AddProperty(nameof(Number))
                .SetNotifier(Client)
                .BindToProperty(x => x.Number)
                .End();
            CreateNotificationBinding()
                .AddProperty(nameof(BatteryChargeLevel))
                .SetNotifier(Client)
                .BindToProperty(x => x.BatteryChargeLevel)
                .End();
            CreateNotificationBinding()
                .AddProperty(nameof(BatteryVoltage))
                .SetNotifier(Client)
                .BindToProperty(x => x.BatteryVoltage)
                .End();
        }

        public RemoteClient Client { get; private set; }

        public bool Connected => connection.Connected;

        public string Name => Client.Name;

        public int Number => Client.Number;

        public string BatteryChargeLevel => $"{Client.BatteryChargeLevel}%";

        public string BatteryVoltage => $"{Math.Round(Client.BatteryVoltage, 2)}V";

        public string ClientState => $"{Client.ClientState}";
    }
}
