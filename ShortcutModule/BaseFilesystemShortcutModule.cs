using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.Api.Utils;

namespace ShortcutModule
{
    public abstract class BaseFilesystemShortcutModule : IZModule
    {
        // Private types ------------------------------------------------------

        private sealed class ShortcutDirectoryInfo : IDisposable
        {
            private readonly FileSystemWatcher watcher;
            private IReadOnlyList<ShortcutInfo> shortcuts;
            private readonly object lockObject = new object();

            public ShortcutDirectoryInfo(string path, bool recursive)
            {
                this.Path = path;
                this.Recursive = recursive;

                watcher = new FileSystemWatcher(path);
                watcher.IncludeSubdirectories = recursive;
                watcher.Changed += OnChanged;
                watcher.Created += OnCreated;
                watcher.Deleted += OnDeleted;
                watcher.Renamed += OnRenamed;
                watcher.EnableRaisingEvents = true;
            }

            private void OnRenamed(object sender, RenamedEventArgs e)
            {
                lock(lockObject)
                {
                    shortcuts = null;
                }
            }

            private void OnDeleted(object sender, FileSystemEventArgs e)
            {
                lock (lockObject)
                {
                    shortcuts = null;
                }
            }

            private void OnCreated(object sender, FileSystemEventArgs e)
            {
                lock (lockObject)
                {
                    shortcuts = null;
                }
            }

            private void OnChanged(object sender, FileSystemEventArgs e)
            {
                lock (lockObject)
                {
                    shortcuts = null;
                }
            }

            public void Dispose()
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }

            public string Path { get; }
            
            public bool Recursive { get; }

            public IReadOnlyList<ShortcutInfo> Shortcuts
            {
                get
                {
                    lock(lockObject)
                    {
                        return shortcuts;
                    }
                }

                set
                {
                    lock(lockObject)
                    {
                        shortcuts = value;
                    }
                }
            }
        }

        // Protected types ----------------------------------------------------

        protected class ShortcutInfo
        {
            public ShortcutInfo(string display, string comment, string command, string parameters, string path)
            {
                this.Display = display;
                this.Comment = Comment;
                this.Command = command;
                this.Path = path;
                this.Parameters = parameters;
            }

            public string Display { get; private set; }
            public string Comment { get; private set; }
            public string Command { get; private set; }
            public string Parameters { get; private set; }
            public string Path { get; private set; }
        }

        private readonly List<ShortcutDirectoryInfo> shortcutDirectories;

        // Private methods ----------------------------------------------------

        private void LoadShortcutsFromRecursive(string path, bool recursive, List<ShortcutInfo> result)
        {
            foreach (var file in Directory.EnumerateFiles(path, "*.lnk"))
            {
                try
                {
                    string name = Path.GetFileNameWithoutExtension(file);

                    WshShell shell = new WshShell();
                    IWshShortcut link = (IWshShortcut)shell.CreateShortcut(file);

                    result.Add(new ShortcutInfo(name, link.Description, link.TargetPath, link.Arguments, file));
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
                            LoadShortcutsFromRecursive(Path.Combine(path, folder), recursive, result);
                    }
                    catch
                    {
                        // Explicitly ignore this folder
                    }
                }
            }
        }

        private List<ShortcutInfo> LoadShortcutsFrom(string path, bool recursive)
        {
            List<ShortcutInfo> result = new List<ShortcutInfo>();

            LoadShortcutsFromRecursive(path, recursive, result);

            return result;
        }

        private IEnumerable<ShortcutInfo> RetrieveShortcutsFrom(ShortcutDirectoryInfo d)
        {
            IEnumerable<ShortcutInfo> shortcuts = d.Shortcuts;

            if (shortcuts == null)
            {
                var loadedShortcuts = LoadShortcutsFrom(d.Path, d.Recursive);
                d.Shortcuts = loadedShortcuts;
                shortcuts = loadedShortcuts;
            }

            return shortcuts;
        }


        // Protected methods --------------------------------------------------

        protected BaseFilesystemShortcutModule()
        {
            var directories = GetShortcutDirectories();

            shortcutDirectories = new List<ShortcutDirectoryInfo>();

            foreach (var directory in directories)
            {
                shortcutDirectories.Add(new ShortcutDirectoryInfo(directory.path, directory.recursive));
            }
        }

        protected abstract List<(string path, bool recursive)> GetShortcutDirectories();

        // Protected properties -----------------------------------------------

        protected abstract string ActionKeyword { get; }
        protected abstract string ActionName { get; }
        protected abstract string ActionDisplay { get; }
        protected abstract string ActionComment { get; }

        protected abstract string FormatError(string errorText);

        // Public methods -----------------------------------------------------

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            Func<ShortcutInfo, bool> filter;
            if (perfectMatchesOnly)
                filter = si => si.Display.ToUpper() == enteredText.ToUpper();
            else
                filter = si => si.Display.ToUpper().Contains(enteredText.ToUpper());        

            shortcutDirectories.SelectMany(d => RetrieveShortcutsFrom(d))
                .Where(filter)
                .OrderBy(s => s.Display)
                .Select(s => new SuggestionInfo(s.Display, s.Display, s.Comment, Icon, SuggestionUtils.EvalMatch(enteredText, s.Display), s))
                .ToList()
                .ForEach(collector.AddSuggestion);
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            var shortcut = shortcutDirectories.SelectMany(d => RetrieveShortcutsFrom(d))
                .FirstOrDefault(s => s.Display.ToUpper() == expression.ToUpper());

            if (shortcut != null)
            {
                try
                {
                    Process.Start(shortcut.Path);
                }
                catch (Exception e)
                {
                    options.ErrorText = FormatError(e.Message);
                    options.PreventClose = true;
                }
            }
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            ShortcutInfo shortcut = suggestion.Data as ShortcutInfo;
            try
            {
                Process.Start(shortcut.Path);
            }
            catch (Exception e)
            {
                options.ErrorText = FormatError(e.Message);
                options.PreventClose = true;
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
