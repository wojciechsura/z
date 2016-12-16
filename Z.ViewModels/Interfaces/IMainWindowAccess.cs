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
        void OpenConfiguration();
        void ShowList();
        void HideList();

        int CaretPosition { get; set; }
        Point Position { get; set; }

    }
}
