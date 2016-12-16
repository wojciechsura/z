using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.BusinessLogic.Interfaces
{
    public interface IMainWindowLogic
    {
        void EnteredTextChanged();
        bool EscapePressed();
        bool SpacePressed();
        bool BackspacePressed();
        bool TabPressed();
        bool EnterPressed();
        bool UpPressed();
        bool DownPressed();
        void WindowLostFocus();

        IMainWindowViewModelAccess MainWindowViewModel { set; }

        bool WindowClosing();
        void WindowInitialized();
        void OpenConfigurationPressed();
    }
}
