using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.Api.Interfaces;
using Z.Api.Types;

namespace StartMenuModule
{
    public class Module : IZModule
    {
        // Private types ------------------------------------------------------

        private class ShortcutInfo
        {
            public ShortcutInfo(string display, string comment, string command, string parameters)
            {
                this.Display = display;
                this.Comment = Comment;
                this.Command = command;
                this.Parameters = parameters;
            }

            public string Display { get; private set; }
            public string Comment { get; private set; }
            public string Command { get; private set; }
            public string Parameters { get; private set; }
        }

        private const string MODULE_DISPLAY_NAME = "Start menu";
        private const string MODULE_NAME = "StartMenu";

        private const string ACTION_KEYWORD = "start";
        private const string ACTION_NAME = "Start";
        private const string ACTION_DISPLAY = "Start menu";

        // Private fields -----------------------------------------------------

        private List<ShortcutInfo> startMenuShortcuts;
        private readonly ImageSource icon;

        // Private methods ----------------------------------------------------

        private void DoRecursiveLoadShortcutsFrom(string path, List<ShortcutInfo> result)
        {
            foreach (var file in Directory.EnumerateFiles(path, "*.lnk"))
            {
                try
                {
                    string name = Path.GetFileNameWithoutExtension(file);

                    WshShell shell = new WshShell();
                    IWshShortcut link = (IWshShortcut)shell.CreateShortcut(file);

                    result.Add(new ShortcutInfo(name, link.Description, link.TargetPath, link.Arguments));
                }
                catch
                {
                    // Explicitly ignore this link
                }
            }

            foreach (var folder in Directory.EnumerateDirectories(path))
            {
                try
                {
                    if (folder != "." && folder != "..")
                        DoRecursiveLoadShortcutsFrom(Path.Combine(path, folder), result);
                }
                catch
                {
                    // Explicitly ignore this folder
                }
            }
        }

        private IEnumerable<ShortcutInfo> RecursiveLoadShortcutsFrom(string path)
        {
            List<ShortcutInfo> result = new List<ShortcutInfo>();

            DoRecursiveLoadShortcutsFrom(path, result);

            return result;
        }

        private void LoadShortcuts()
        {
            string startMenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            string commonStartMenu = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);

            startMenuShortcuts = RecursiveLoadShortcutsFrom(startMenu)
                .Union(RecursiveLoadShortcutsFrom(commonStartMenu))
                .ToList();
        }

        // Public methods -----------------------------------------------------

        public Module()
        {
            LoadShortcuts();
            icon = new BitmapImage(new Uri("pack://application:,,,/StartMenuModule;component/Resources/winlogo.png"));
        }

        public void CollectSuggestions(string enteredText, string keywordAction, ISuggestionCollector collector)
        {
            startMenuShortcuts
                .Where(s => s.Display.ToUpper().Contains(enteredText.ToUpper()))
                .OrderBy(s => s.Display)
                .Select(s => new SuggestionInfo(s.Display, s.Display, s.Comment, icon, s))
                .ToList()
                .ForEach(s => collector.AddSuggestion(s));
        }

        public void ExecuteKeywordAction(string action, string expression)
        {
            var shortcut = startMenuShortcuts
                .Where(s => s.Display.ToUpper() == expression.ToUpper())
                .FirstOrDefault();

            if (shortcut != null)
            {
                try
                {
                    Process.Start(shortcut.Command, shortcut.Parameters);
                }
                catch
                {

                }
            }
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion)
        {
            ShortcutInfo shortcut = suggestion.Data as ShortcutInfo;
            try
            {
                Process.Start(shortcut.Command, shortcut.Parameters);
            }
            catch
            {

            }        
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(ACTION_KEYWORD, ACTION_NAME, ACTION_DISPLAY);
        }

        // Public properties --------------------------------------------------

        public string DisplayName
        {
            get
            {
                return MODULE_DISPLAY_NAME;
            }
        }

        public string InternalName
        {
            get
            {
                return MODULE_NAME;
            }
        }
    }
}
