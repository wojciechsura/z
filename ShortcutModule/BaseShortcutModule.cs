using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;

namespace ShortcutModule
{
    public abstract class BaseShortcutModule : IZModule
    {
        // Protected types ----------------------------------------------------

        protected class ShortcutInfo
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

        private List<ShortcutInfo> shortcuts;

        // Private methods ----------------------------------------------------

        private void DoLoadShortcutsFrom(string path, bool recursive, List<ShortcutInfo> result)
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

            if (recursive)
            {
                foreach (var folder in Directory.EnumerateDirectories(path))
                {
                    try
                    {
                        if (folder != "." && folder != "..")
                            DoLoadShortcutsFrom(Path.Combine(path, folder), recursive, result);
                    }
                    catch
                    {
                        // Explicitly ignore this folder
                    }
                }
            }
        }

        // Protected properties -----------------------------------------------

        protected abstract string ActionKeyword { get; }
        protected abstract string ActionName { get; }
        protected abstract string ActionDisplay { get; }
        protected abstract string ActionComment { get; }

        // Protected methods --------------------------------------------------

        protected IEnumerable<ShortcutInfo> LoadShortcutsFrom(string path, bool recursive)
        {
            List<ShortcutInfo> result = new List<ShortcutInfo>();

            DoLoadShortcutsFrom(path, recursive, result);

            return result;
        }

        protected abstract List<ShortcutInfo> LoadShortcuts();

        // Public methods -----------------------------------------------------

        public BaseShortcutModule()
        {
            shortcuts = LoadShortcuts();
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            Func<ShortcutInfo, bool> filter;
            if (perfectMatchesOnly)
                filter = si => si.Display.ToUpper() == enteredText.ToUpper();
            else
                filter = si => si.Display.ToUpper().Contains(enteredText.ToUpper());        

            shortcuts
                .Where(filter)
                .OrderBy(s => s.Display)
                .Select(s => new SuggestionInfo(s.Display, s.Display, s.Comment, Icon, s))
                .ToList()
                .ForEach(s => collector.AddSuggestion(s));
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            var shortcut = shortcuts
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

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
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
            yield return new KeywordInfo(ActionKeyword, ActionName, ActionDisplay, ActionComment);
        }

        // Public properties --------------------------------------------------

        public abstract string Name { get; }
        public abstract string DisplayName { get; }

        public abstract ImageSource Icon { get; }
    }
}
