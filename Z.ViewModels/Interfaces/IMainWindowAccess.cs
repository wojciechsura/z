using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.ViewModels.Interfaces
{
    public interface IMainWindowAccess
    {
        void Show();
        void Hide();
        void ShowError(string error);
        void Shutdown();

        int CaretPosition { get; set; }
    }
}
