using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Viewmodels.Interfaces
{
    public interface IMainViewModelAccess
    {
        void Show();
        void Hide();
        void ShowError(string error);
        void Shutdown();

        int CaretPosition { get; set; }
    }
}
