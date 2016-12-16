using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Z.BusinessLogic.Interfaces
{
    public interface IMainWindowViewModelAccess
    {
        string EnteredText { get; set; }
        string Keyword { set; }
        bool KeywordVisible { set; }
        int CaretPosition { get; set; }

        void ShowWindow();
        void HideWindow();
        void ShowList();
        void HideList();
        Point Position { get; set; }
        bool ShowHint { get; set; }
    }
}
