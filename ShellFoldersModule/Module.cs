using Microsoft.Win32;
using ShellFoldersModule.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api;
using System.Windows.Media;
using Z.Api.Interfaces;
using Z.Api.Types;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Shell;
using System.Windows.Media.Imaging;

namespace ShellFoldersModule
{
    public class Module : IZModule
    {
        private class ShellFolderInfo
        {
            public ShellFolderInfo(string key, string display, string comment, string command)
            {
                Key = key;
                Display = display;
                Comment = comment;
                CanonicalName = command;
            }

            public string Key { get; private set; }
            public string Display { get; private set; }
            public string Comment { get; private set; }
            public string CanonicalName { get; private set; }
        }

        private const string MODULE_NAME = "ShellFolders";
        private const string MODULE_DISPLAY_NAME = "Shell folder";

        private const string SHELL_ACTION_KEYWORD = "shell";
        private const string SHELL_ACTION_NAME = "ShellFolder";
        private const string SHELL_ACTION_DISPLAY = "Shell folder";
        private const string SHELL_ACTION_COMMENT = "Open shell folder";

        private List<ShellFolderInfo> infos;
        private ImageSource icon;

        private void LoadShellFoldersFromRegistry()
        {
            // Shell folders from registry

            var shellFolders = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders");
            foreach (string name in shellFolders.GetValueNames())
            {
                var folder = shellFolders.GetValue(name) as string;

                var fullPath = WinApiInterop.GetFullPath(folder);
                var key = Path.GetFileName(fullPath);
                var displayName = WinApiInterop.GetLocalizedName(fullPath);

                infos.Add(new ShellFolderInfo(key, displayName, fullPath, fullPath));
            }
        }

        private static string GenerateLocalizedName(IKnownFolder shellFolder)
        {
            string localizedName = shellFolder.LocalizedName;

            if (String.IsNullOrEmpty(localizedName) && Directory.Exists(shellFolder.Path))
            {
                try
                {
                    localizedName = WinApiInterop.GetLocalizedName(shellFolder.Path);
                }
                catch
                {

                }
            }

            if (String.IsNullOrEmpty(localizedName) && Directory.Exists(shellFolder.Path))
                localizedName = Path.GetFileName(shellFolder.Path);

            if (String.IsNullOrEmpty(localizedName))
                localizedName = shellFolder.CanonicalName;
            return localizedName;
        }

        private void LoadShellFolders()
        {
            foreach (var shellFolder in KnownFolders.All)
            {
                string localizedName = GenerateLocalizedName(shellFolder);

                string comment = shellFolder.PathExists ? shellFolder.Path : $"shell:{shellFolder.CanonicalName}";

                infos.Add(new ShellFolderInfo(shellFolder.CanonicalName,
                    localizedName,
                    comment,
                    shellFolder.CanonicalName));
            }
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            Func<ShellFolderInfo, bool> func;

            if (perfectMatchesOnly)
                func = (sfi => sfi.Key.ToUpper() == enteredText.ToUpper() || sfi.Display.ToUpper() == enteredText.ToUpper());
            else
                func = sfi => sfi.Key.ToUpper().Contains(enteredText.ToUpper()) || sfi.Display.ToUpper().Contains(enteredText.ToUpper());

            infos.Where(func)
                .OrderBy(sfi => sfi.Display)
                .ToList()
                .ForEach(sfi => collector.AddSuggestion(new SuggestionInfo(sfi.Key, sfi.Display, sfi.Comment, icon, sfi.CanonicalName)));
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            var info = infos.Where(i => i.Key.ToUpper() == expression.ToUpper() || i.Display.ToUpper() == expression.ToUpper());
            if (info.Count() == 1)
            {
                try
                {
                    Process.Start($"shell:{info.First().CanonicalName}");
                }
                catch
                {
                    // TODO notify about error
                    options.PreventClose = true;
                }
            }
            else
            {
                // TODO notify about error
                options.PreventClose = true;
            }
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            try
            {
                string canonicalName = suggestion.Data as string;
                Process.Start($"shell:{canonicalName}");
            }
            catch
            {
                // TODO notify about error
                options.PreventClose = true;
            }
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(SHELL_ACTION_KEYWORD, SHELL_ACTION_NAME, SHELL_ACTION_DISPLAY, SHELL_ACTION_COMMENT);
        }

        public Module()
        {
            infos = new List<ShellFolderInfo>();
            LoadShellFolders();

            icon = new BitmapImage(new Uri("pack://application:,,,/ShellFoldersModule;component/Resources/shell.png"));
        }

        public string Name
        {
            get
            {
                return MODULE_NAME;
            }
        }

        public string DisplayName
        {
            get
            {
                return MODULE_DISPLAY_NAME;
            }
        }

        public ImageSource Icon
        {
            get
            {
                return icon;
            }
        }
    }
}
