using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Z.BusinessLogic.Interfaces.ViewModels
{
    public interface IMainWindowViewModel
    {
        string Keyword { get; }
        bool KeywordVisible { get; }
        string EnteredText { get; set; }

        bool EscapePressed();
        bool SpacePressed();
        bool BackspacePressed();
        bool TabPressed();
        bool EnterPressed();
        void WindowLostFocus();
        bool UpPressed();
        bool DownPressed();
        bool Closing();
        void Initialized();

        IMainWindowAccess MainWindowAccess { set; }
        ICommand ConfigurationCommand { get; }
        bool ShowHint { get; }
    }
}
