using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.BusinessLogic.Services.AppWindows
{
    public interface IAppWindowService
    {
        void ShowMainWindow();
        void HideMainWindow();
        void ToggleMainWindow();
        void ShowProCalcWindow();
        void HideProCalcWindow();
    }
}
