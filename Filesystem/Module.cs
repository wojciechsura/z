using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Interfaces;
using Z.Api.Types;

namespace Filesystem
{
    public class Module : IZModule
    {
        private const string MODULE_DISPLAY_NAME = "Filesystem";
        private const string MODULE_NAME = "Filesystem";

        private const string FILE_KEYWORD = "file";
        private const string FILE_ACTION = "File";
        private const string FILE_KEYWORD_DISPLAY = "File";

        private const int LONG_FILENAME = 60;

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

        public void CollectSuggestions(string enteredText, string keywordAction, ISuggestionCollector collector)
        {
            string dir, search;

            if (enteredText.EndsWith("\\"))
            {
                dir = enteredText;
                search = "*";
            }
            else
            {
                try
                {
                    dir = Path.GetDirectoryName(enteredText);
                    search = $"*{Path.GetFileName(enteredText)}*";
                }
                catch
                {
                    // Entered text (mostly likely) is not a path
                    return;
                }
            }
            
            if (Directory.Exists(dir))
            {
                try
                {
                    // Directories
                    foreach (var file in Directory.EnumerateDirectories(dir, search))
                    {
                        string display = file.Length < LONG_FILENAME ? file : $"...{file.Substring(file.Length - LONG_FILENAME)}";
                        collector.AddSuggestion(new SuggestionInfo(file, display, "Folder"));
                    }

                    // Files
                    foreach (var file in Directory.EnumerateFiles(dir, search))
                    {
                        string display = file.Length < LONG_FILENAME ? file : $"...{file.Substring(file.Length - LONG_FILENAME)}";
                        collector.AddSuggestion(new SuggestionInfo(file, file, "File"));
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        public void ExecuteKeywordAction(string action, string expression)
        {
            Process.Start(expression);
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(FILE_KEYWORD, FILE_ACTION, FILE_KEYWORD_DISPLAY);
        }
    }
}
