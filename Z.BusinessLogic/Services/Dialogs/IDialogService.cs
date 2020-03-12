using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Models;

namespace Z.BusinessLogic.Services.Dialogs
{
    public interface IDialogService
    {
        OpenDialogResult ShowOpenDialog(string filter = null, string title = null, string filename = null, string path = null);
        SaveDialogResult ShowSaveDialog(string filter = null, string title = null, string filename = null, string path = null);
    }
}
