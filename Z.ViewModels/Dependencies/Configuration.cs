using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.ViewModels;
using Z.ViewModels.Interfaces;

namespace Z.ViewModels.Dependencies
{
    public static class Configuration
    {
        public static void Configure(IUnityContainer container)
        {
            BusinessLogic.Dependencies.Configuration.Configure(container);

            container.RegisterType<IViewModelFactory, ViewModelFactory>();
        }
    }
}
