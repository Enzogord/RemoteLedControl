﻿using System.Reflection;
using Autofac;
using Core;
using RLCCore;
using RLCCore.Settings;
using RLCServerApplication.ViewModels;

namespace RLCServerApplication
{
    public static class Bootstrapper
    {
        private static ILifetimeScope rootScope;

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

            builder.RegisterType<MainWindowViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<SequencePlayer>().InstancePerLifetimeScope();
            builder.RegisterType<RLCProjectController>().InstancePerLifetimeScope();
            builder.RegisterType<NetworkController>().InstancePerLifetimeScope();

            rootScope = builder.Build();
        }

        public static void Stop()
        {
            rootScope.Dispose();
            rootScope = null;
        }
    }
}
