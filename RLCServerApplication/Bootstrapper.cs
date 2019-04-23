using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using RLCCore;
using RLCServerApplication.ViewModels;

namespace RLCServerApplication
{
    public static class Bootstrapper
    {
        private static ILifetimeScope rootScope;
        public static ILifetimeScope RootScope => rootScope;

        public static MainWindowViewModel RootViewModel {
            get {
                if(rootScope == null) {
                    Start();
                }
                return rootScope.Resolve<MainWindowViewModel>();
            }
        }

        public static void Start()
        {
            if(rootScope != null) {
                return;
            }

            var assembly = Assembly.GetExecutingAssembly();

            var builder = new ContainerBuilder();

            builder.RegisterType<MainWindowViewModel>();
            builder.RegisterType<RLCProjectController>().SingleInstance();
            builder.RegisterType<SettingsViewModel>();
            builder.RegisterType<RemoteClientsViewModel>();
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Service"));

            rootScope = builder.Build();
        }

        public static void Stop()
        {
            rootScope.Dispose();
            rootScope = null;
        }
    }
}
