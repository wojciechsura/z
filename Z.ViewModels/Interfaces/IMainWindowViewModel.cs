﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.ViewModels.Interfaces
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

        IMainWindowAccess MainWindowAccess { set; }

        bool UpPressed();
        bool DownPressed();
    }
}
