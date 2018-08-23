using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Services.Interfaces;

namespace Z.Services
{
    class WindowService : IWindowService
    {
        private MainWindow mainWindow;

        public WindowService()
        {
            mainWindow = new MainWindow();
        }

        public void HideMainWindow()
        {
            mainWindow.Dismiss();
        }

        public void ShowMainWindow()
        {
            mainWindow.Summon();
        }

        public void ToggleMainWindow()
        {
            if (mainWindow.Visibility == System.Windows.Visibility.Visible)
                HideMainWindow();
            else
                ShowMainWindow();
        }
    }
}
