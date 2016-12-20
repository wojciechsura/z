using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
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
        private const string FILE_KEYWORD_COMMENT = "Browse through the filesystem";

        private const int LONG_FILENAME = 60;
        private BitmapImage folderImage;
        private BitmapImage fileImage;

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

        public void CollectSuggestions(string enteredText, string keywordAction, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            string dir, search;

            if (perfectMatchesOnly && Directory.Exists(enteredText))
            {
                collector.AddSuggestion(new SuggestionInfo(enteredText, enteredText, null, folderImage));
                return;
            }
            else if (perfectMatchesOnly && File.Exists(enteredText))
            {
                collector.AddSuggestion(new SuggestionInfo(enteredText, enteredText, null, fileImage));
                return;
            }

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
                        collector.AddSuggestion(new SuggestionInfo(file, display, null, folderImage));
                    }

                    // Files
                    foreach (var file in Directory.EnumerateFiles(dir, search))
                    {
                        string display = file.Length < LONG_FILENAME ? file : $"...{file.Substring(file.Length - LONG_FILENAME)}";
                        collector.AddSuggestion(new SuggestionInfo(file, file, null, fileImage));
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            Process.Start(expression);
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            Process.Start(suggestion.Text);
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(FILE_KEYWORD, FILE_ACTION, FILE_KEYWORD_DISPLAY, FILE_KEYWORD_COMMENT);
        }

        public Module()
        {
            fileImage = new BitmapImage(new Uri("pack://application:,,,/Filesystem;component/Resources/file.png"));
            folderImage = new BitmapImage(new Uri("pack://application:,,,/Filesystem;component/Resources/folder.png"));
        }
    }
}
