using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
