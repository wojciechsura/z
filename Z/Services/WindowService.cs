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
        private ProCalcWindow proCalcWindow;

        public WindowService()
        {
            mainWindow = new MainWindow();
            proCalcWindow = new ProCalcWindow();
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

        public void HideProCalcWindow()
        {
            proCalcWindow.Dismiss();
        }

        public void ShowProCalcWindow()
        {
            proCalcWindow.Summon();
        }

        public void ToggleProCalcWindow()
        {
            if (proCalcWindow.Visibility == System.Windows.Visibility.Visible)
                HideProCalcWindow();
            else
                ShowProCalcWindow();
        }
    }
}
