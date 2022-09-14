using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Services.Application;
using Z.BusinessLogic.Services.AppWindows;
using Z.BusinessLogic.Services.Dialogs;
using Z.BusinessLogic.Services.Image;
using Z.BusinessLogic.Services.Messaging;
using Z.Services;
using Z.Services.AppWindows;
using Z.Services.Dialogs;
using Z.Services.Image;
using Z.Services.Messaging;

namespace Z.Dependencies
{
    public static class Configuration
    {
        public static void Configure(ContainerBuilder builder, App app)
        {
            builder.RegisterInstance<IApplicationController>(app);
            builder.RegisterType<AppWindowService>().As<IAppWindowService>().SingleInstance();
            builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
            builder.RegisterType<MessagingService>().As<IMessagingService>().SingleInstance();
            builder.RegisterType<ImageResources>().As<IImageResources>().SingleInstance();

            BusinessLogic.Dependencies.Configuration.Configure(builder);
        }
    }
}
