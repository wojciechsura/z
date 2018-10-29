using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Z.BusinessLogic.ViewModels;
using Z.Dependencies;
using Microsoft.Practices.Unity;
using Z.BusinessLogic.Services.Interfaces;
using Z.BusinessLogic.ViewModels.Interfaces;

namespace Z
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IApplicationController, IApplicationAccess
    {
        private Mutex singleInstanceMutex;
        private TaskbarIcon taskbarIcon;

        private AppViewModel viewModel;

        private void InitializeTaskbarIcon()
        {
            taskbarIcon = new TaskbarIcon();
            taskbarIcon.Icon = new Icon(GetResourceStream(new Uri(@"pack://application:,,,/Z.ico")).Stream);
            taskbarIcon.LeftClickCommand = viewModel.TaskbarClickCommand;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bool isNewInstance = false;
            singleInstanceMutex = new Mutex(true, "Spooksoft.ZLauncher", out isNewInstance);
            if (!isNewInstance)
            {
                MessageBox.Show("Z launcher is already running!");
                App.Current.Shutdown();
                return;
            }

            Configuration.Configure(this);
            Configuration.LateConfigure();

            viewModel = Dependencies.Container.Instance.Resolve<AppViewModel>();
            viewModel.ApplicationAccess = this;

            InitializeTaskbarIcon();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            singleInstanceMutex.Dispose();
            Configuration.Dispose();

            base.OnExit(e);
        }

        void IApplicationController.Shutdown()
        {
            viewModel.Shutdown();
        }

        void IApplicationAccess.Shutdown()
        {
            this.Shutdown();
        }
    }
}
