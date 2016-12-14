using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Z.ViewModels.Interfaces
{
    public interface IMainWindowAccess
    {
        void Show();
        void Hide();

        int CaretPosition { get; set; }

        void ShowList();
        void HideList();
        Point Position { get; set; }
    }
}
