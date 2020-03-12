using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Services.Application;
using Z.BusinessLogic.Services.AppWindows;
using Z.BusinessLogic.Services.Dialogs;
using Z.BusinessLogic.Services.Messaging;
using Z.Services;
using Z.Services.AppWindows;
using Z.Services.Dialogs;
using Z.Services.Messaging;

namespace Z.Dependencies
{
    public static class Configuration
    {
        public static void Configure(App app)
        {
            Container.Instance.RegisterInstance<IApplicationController>(app);
            Container.Instance.RegisterType<IAppWindowService, AppWindowService>(new ContainerControlledLifetimeManager());
            Container.Instance.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            Container.Instance.RegisterType<IMessagingService, MessagingService>(new ContainerControlledLifetimeManager());

            BusinessLogic.Dependencies.Configuration.Configure(Container.Instance);
        }

        public static void Dispose()
        {
            Container.Dispose();
        }

        internal static void LateConfigure()
        {
            BusinessLogic.Dependencies.Configuration.LateConfigure(Container.Instance);
        }
    }
}
