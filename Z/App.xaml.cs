using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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
        private Mutex singleInstanceMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bool isNewInstance = false;
            singleInstanceMutex = new Mutex(true, "Spooksoft.ZLauncher", out isNewInstance);
            if (!isNewInstance)
            {
                MessageBox.Show("Z launcher is already running!");
                App.Current.Shutdown();
            }

            Configuration.Configure();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            singleInstanceMutex.Dispose();
            Configuration.Dispose();

            base.OnExit(e);
        }
    }
}
