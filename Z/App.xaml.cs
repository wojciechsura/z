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
using System.Windows.Threading;
using System.Reflection;
using Z.BusinessLogic.ViewModels.Application;
using Z.BusinessLogic.Services.Application;
using Z.Resources;

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

            Application.Current.DispatcherUnhandledException += HandleUncaughtException;

            bool isNewInstance = false;
            singleInstanceMutex = new Mutex(true, "Spooksoft.ZLauncher", out isNewInstance);
            if (!isNewInstance)
            {
                MessageBox.Show(Strings.Z_Message_ZLauncherIsAlreadyRunning);
                App.Current.Shutdown();
                return;
            }

            Configuration.Configure(this);
            Configuration.LateConfigure();

            viewModel = Dependencies.Container.Instance.Resolve<AppViewModel>();
            viewModel.ApplicationAccess = this;

            InitializeTaskbarIcon();
        }

        private void HandleUncaughtException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string localRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!localRoaming.EndsWith("\\"))
                localRoaming += '\\';

            string appDataPath = $"{localRoaming}Spooksoft\\Z\\Log\\";
            System.IO.Directory.CreateDirectory(appDataPath);

            string logPath = System.IO.Path.Combine(appDataPath, "error.log");

            string error = e.Exception.ToString();

            if (!System.IO.File.Exists(logPath))
                System.IO.File.WriteAllText(logPath, "");

            var log = $"-------------------------------------------------------\n{DateTime.Now}\n\n{error}";

            System.IO.File.AppendAllText(logPath, log);

            e.Handled = false;
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
