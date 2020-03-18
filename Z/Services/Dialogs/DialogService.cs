using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BusinessLogic.Models;
using Z.BusinessLogic.Services.Dialogs;
using Z.Resources;

namespace Z.Services.Dialogs
{
    class DialogService : IDialogService
    {
        private void SetupFileDialog(FileDialog dialog, string filter, string title, string filename, string path)
        {
            if (filename != null)
                dialog.FileName = filename;

            if (filter != null)
                dialog.Filter = filter;
            else
                dialog.Filter = Strings.Z_DefaultFilter;

            if (title != null)
                dialog.Title = title;
            else
                dialog.Title = Strings.Z_DefaultDialogTitle;

            if (path != null)
                dialog.InitialDirectory = path;
        }

        public OpenDialogResult ShowOpenDialog(string filter = null, string title = null, string filename = null, string path = null)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            SetupFileDialog(dialog, filter, title, filename, path);

            if (dialog.ShowDialog() == true)
                return new OpenDialogResult(true, dialog.FileName);
            else
                return new OpenDialogResult(false, null);
        }

        public SaveDialogResult ShowSaveDialog(string filter = null, string title = null, string filename = null, string path = null)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            SetupFileDialog(dialog, filter, title, filename, path);

            if (dialog.ShowDialog() == true)
                return new SaveDialogResult(true, dialog.FileName);
            else
                return new SaveDialogResult(false, null);
        }
    }
}
