using RLCCore.Domain;
using RLCCore.Settings;
using SNTPService;
using System;

namespace Core
{
    public class SntpServer
    {
        private readonly SntpService sntpService;
        private readonly INetworkSettingProvider networkSettingProvider;
        private readonly RemoteControlProject project;

        public SntpServer(SntpService sntpService, INetworkSettingProvider networkSettingProvider, RemoteControlProject project)
        {
            this.sntpService = sntpService ?? throw new ArgumentNullException(nameof(sntpService));
            this.networkSettingProvider = networkSettingProvider ?? throw new ArgumentNullException(nameof(networkSettingProvider));
            this.project = project ?? throw new ArgumentNullException(nameof(project));
        }

        public void Start()
        {
            sntpService.Start(networkSettingProvider.GetServerIPAddress(), project.SntpPort);
        }

        public void Stop()
        {
            sntpService.Stop();
        }
    }
}
