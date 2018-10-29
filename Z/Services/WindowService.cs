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
        private Lazy<MainWindow> mainWindow;
        private Lazy<ProCalcWindow> proCalcWindow;

        public WindowService()
        {
            mainWindow = new Lazy<MainWindow>(() => new MainWindow());
            proCalcWindow = new Lazy<ProCalcWindow>(() => new ProCalcWindow());
        }

        public void HideMainWindow()
        {
            mainWindow.Value.Dismiss();
        }

        public void ShowMainWindow()
        {
            mainWindow.Value.Summon();

            if (proCalcWindow.Value.IsVisible)
                proCalcWindow.Value.Dismiss();
        }

        public void ToggleMainWindow()
        {
            if (mainWindow.Value.IsVisible)
                HideMainWindow();
            else if (proCalcWindow.Value.IsVisible)
                HideProCalcWindow();
            else
                ShowMainWindow();
        }

        public void HideProCalcWindow()
        {
            proCalcWindow.Value.Dismiss();
        }

        public void ShowProCalcWindow()
        {
            proCalcWindow.Value.Summon();

            if (mainWindow.Value.IsVisible)
                mainWindow.Value.Dismiss();
        }
    }
}
