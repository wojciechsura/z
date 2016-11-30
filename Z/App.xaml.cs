using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Z.Dependencies;

namespace Z
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Configuration.Configure();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Configuration.Dispose();

            base.OnExit(e);
        }
    }
}
