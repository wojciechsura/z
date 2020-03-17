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
using System.Windows.Media;
using Z.Api;
using Z.Api.Utils;
using Filesystem.Models;
using Filesystem.Types;
using System.ComponentModel;
using System.Xml.Serialization;
using Filesystem.ViewModels;
using Filesystem.Windows;
using Filesystem.Resources;

namespace Filesystem
{
    public class Module : IZModule, IZInitializable, IZConfigurable
    {
        private class ConfigurationProvider : IConfigurationProvider
        {
            private readonly Configuration configuration;
            private ConfigurationWindowViewModel viewModel;

            public ConfigurationProvider(Configuration configuration)
            {
                this.configuration = configuration;
                viewModel = new ConfigurationWindowViewModel(configuration);
            }

            public void Dispose()
            {

            }

            public void Save()
            {
                viewModel.Save();
            }

            public void Show()
            {
                var window = new ConfigurationWindow(viewModel);
                window.ShowDialog();
            }

            public IEnumerable<string> Validate()
            {
                return viewModel.Validate();
            }
        }

        private const string MODULE_NAME = "Filesystem";

        private const string FILE_KEYWORD = "file";
        private const string FILE_ACTION = "File";

        private const string CONFIG_FILENAME = "config.xml";

        private const int LONG_FILENAME = 60;
        private BitmapImage folderImage;
        private BitmapImage fileImage;

        private Configuration configuration;
        private IModuleContext context;

        private void RegularCollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            string dir, search;

            if (perfectMatchesOnly && Directory.Exists(enteredText))
            {
                collector.AddSuggestion(new SuggestionInfo(enteredText, enteredText, null, folderImage, 100));
                return;
            }
            else if (perfectMatchesOnly && File.Exists(enteredText))
            {
                collector.AddSuggestion(new SuggestionInfo(enteredText, enteredText, null, fileImage, 100));
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
                        collector.AddSuggestion(new SuggestionInfo(file, display, null, folderImage, 100, null, $"{MODULE_NAME}0"));
                    }

                    // Files
                    foreach (var file in Directory.EnumerateFiles(dir, search))
                    {
                        string display = file.Length < LONG_FILENAME ? file : $"...{file.Substring(file.Length - LONG_FILENAME)}";
                        collector.AddSuggestion(new SuggestionInfo(file, file, null, fileImage, 100, null, $"{MODULE_NAME}1"));
                    }
                }
                catch
                {
                    return;
                }
            }
        }
        
        private void DeepCollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            if (perfectMatchesOnly)
            {
                if (Directory.Exists(enteredText))
                    collector.AddSuggestion(new SuggestionInfo(enteredText, enteredText, null, folderImage, 100));
                else if (File.Exists(enteredText))
                    collector.AddSuggestion(new SuggestionInfo(enteredText, enteredText, null, fileImage, 100));

                return;
            }

            try
            {
                if (Path.IsPathRooted(enteredText))
                {
                    string[] pathElements = enteredText.Split(Path.DirectorySeparatorChar);

                    List<string> search = new List<string> { pathElements[0] + Path.DirectorySeparatorChar.ToString() };

                    for (int i = 1; i < pathElements.Length - 1; i++)
                    {
                        List<string> newSearch = new List<string>();

                        for (int j = 0; j < search.Count; j++)
                        {
                            try
                            {
                                foreach (var dir in Directory.EnumerateDirectories(search[j], $"*{pathElements[i]}*"))
                                {
                                    var path = dir.EndsWith(Path.DirectorySeparatorChar.ToString())
                                        ? dir
                                        : $"{dir}{Path.DirectorySeparatorChar}";

                                    newSearch.Add(path);
                                }
                            }
                            catch
                            {

                            }
                        }

                        search = newSearch;
                    }

                    string searchString = pathElements.Length > 1 ? $"*{pathElements.Last()}*" : "*";

                    for (int i = 0; i < search.Count; i++)
                    {
                        try
                        {

                            // Directories
                            foreach (var file in Directory.EnumerateDirectories(search[i], searchString))
                            {
                                string display = file.Length < LONG_FILENAME
                                    ? file
                                    : $"...{file.Substring(file.Length - LONG_FILENAME)}";
                                collector.AddSuggestion(new SuggestionInfo(file, display, null, folderImage, 100, null,
                                    $"{MODULE_NAME}0"));
                            }

                            // Files
                            foreach (var file in Directory.EnumerateFiles(search[i], searchString))
                            {
                                string display = file.Length < LONG_FILENAME
                                    ? file
                                    : $"...{file.Substring(file.Length - LONG_FILENAME)}";
                                collector.AddSuggestion(new SuggestionInfo(file, file, null, fileImage, 100, null,
                                    $"{MODULE_NAME}1"));
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void LoadConfiguration()
        {
            if (context.ConfigurationFileExists(CONFIG_FILENAME))
                using (FileStream fs = context.OpenConfigurationFile(CONFIG_FILENAME, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                        configuration = (Configuration)serializer.Deserialize(fs);
                    }
                    catch
                    {
                        configuration = new Configuration();
                    }
                }
        }

        private void SaveConfiguration()
        {
            using (FileStream fs = context.OpenConfigurationFile(CONFIG_FILENAME, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                serializer.Serialize(fs, configuration);
            }
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            switch (configuration.FileSearchStrategy)
            {
                case FileSearchStrategy.Deep:
                    {
                        DeepCollectSuggestions(enteredText, action, perfectMatchesOnly, collector);
                        break;
                    }
                case FileSearchStrategy.Direct:
                    {
                        RegularCollectSuggestions(enteredText, action, perfectMatchesOnly, collector);
                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported file search strategy!");
            }                
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            try
            {
                Process.Start(expression);
            }
            catch (Exception e)
            {
                options.ErrorText = string.Format(Strings.Filesystem_Message_CannotExecute, e.Message);
                options.PreventClose = true;
            }
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            try
            {
                Process.Start(suggestion.Text);
            }
            catch (Exception e)
            {
                options.ErrorText = string.Format(Strings.Filesystem_Message_CannotExecute, e.Message);
                options.PreventClose = true;
            }
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(FILE_KEYWORD, FILE_ACTION, Strings.Filesystem_ActionDisplayName, Strings.Filesystem_ActionComment);
        }

        public void Initialize(IModuleContext context)
        {
            this.context = context;
            LoadConfiguration();
        }

        public void Deinitialize()
        {
            SaveConfiguration();
        }

        public IConfigurationProvider GetConfigurationProvider()
        {
            return new ConfigurationProvider(configuration);
        }

        public Module()
        {
            configuration = new Configuration();

            fileImage = new BitmapImage(new Uri("pack://application:,,,/Filesystem;component/Resources/file.png"));
            folderImage = new BitmapImage(new Uri("pack://application:,,,/Filesystem;component/Resources/folder.png"));
        }

        public string DisplayName => Strings.Filesystem_ModuleDisplayName;

        public string Name => MODULE_NAME;

        public ImageSource Icon => folderImage;
    }
}
