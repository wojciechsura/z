using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Z.BusinessLogic.ViewModels.Interfaces
{
    public interface IProCalcWindowAccess
    {
        int CaretPosition { get; set; }
        Point Position { get; set; }
        Point RelativePosition { get; set; }
        bool IsVisible { get; }

        void Show();
        void ShowResults();
        void Hide();
        void OpenConfiguration();
        void InputSelectAll();
    }
}
