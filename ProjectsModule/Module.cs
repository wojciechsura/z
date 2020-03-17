using ProjectsModule.Models;
using ProjectsModule.Resources;
using ProjectsModule.ViewModels;
using ProjectsModule.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.Api.Utils;

namespace ProjectsModule
{
    public class Module : IZModule, IZInitializable, IZConfigurable, IZSuggestionComplete
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

        private const string MODULE_NAME = "Projects";

        private const string PROJECT_KEYWORD = "project";
        private const string PROJECT_ACTION = "Project";

        private const string CONFIG_FILENAME = "config.xml";

        private const int LONG_FILENAME = 60;

        private Configuration configuration;
        private IModuleContext context;
        private ImageSource icon;

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

        private List<string> GetOptionsFor(string text, bool perfectMatchesOnly)
        {
            List<string> result = new List<string>();

            foreach (var root in configuration.ProjectRoots)
            {
                if (Directory.Exists(root))
                {
                    string search;

                    if (perfectMatchesOnly)
                    {
                        search = text;
                    }
                    else
                    {
                        search = $"*{text}*";
                    }

                    try
                    {
                        foreach (var file in Directory.EnumerateDirectories(root, search))
                        {
                            result.Add(file);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return result;
        }

        public Module()
        {
            configuration = new Configuration();
            icon = new BitmapImage(new Uri("pack://application:,,,/ProjectsModule;component/Resources/project.png"));
        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            foreach (var file in GetOptionsFor(enteredText, perfectMatchesOnly))
            {
                var filename = Path.GetFileName(file);
                collector.AddSuggestion(new SuggestionInfo(filename, filename, file, icon, SuggestionUtils.EvalMatch(enteredText, filename), file));
            }
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            List<string> paths = GetOptionsFor(expression, true);
            if (paths.Count == 1)
            {
                Process.Start(paths[0]);
            }
            else
            {
                options.ErrorText = ProjectsModule.Resources.Strings.Projects_Message_MoreThanOneProjectMatchesName;
                options.PreventClose = true;
            }
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            Process.Start(suggestion.Data as string);
        }

        public IConfigurationProvider GetConfigurationProvider()
        {
            return new ConfigurationProvider(configuration);
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(PROJECT_KEYWORD, PROJECT_ACTION, Strings.Projects_ActionDisplayName, Strings.Projects_ActionComment);
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

        public bool CanComplete(string action, SuggestionInfo suggestion)
        {
            return string.IsNullOrEmpty(action);
        }

        public string Complete(string action, SuggestionInfo suggestion)
        {
            if (string.IsNullOrEmpty(action))
                return suggestion.Data as string;
            else
                return null;
        }

        public string DisplayName
        {
            get
            {
                return Strings.Projects_ModuleDisplayName;
            }
        }

        public ImageSource Icon
        {
            get
            {
                return icon;
            }
        }

        public string Name
        {
            get
            {
                return MODULE_NAME;
            }
        }
    }
}
