using CustomCommandsModule.Infrastructure;
using CustomCommandsModule.Models;
using CustomCommandsModule.ViewModels;
using CustomCommandsModule.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.Api.Utils;

namespace CustomCommandsModule
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

        private const string MODULE_DISPLAY_NAME = "Custom";
        private const string MODULE_NAME = "Custom";
        private const string CONFIG_FILENAME = "config.xml";
        private readonly ImageSource icon;
        private Configuration configuration;
        private IModuleContext context;

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

        private CommandParams GetCommandInfo(string command)
        {
            int i = 0;
            bool inQuotes = false;

            while (!inQuotes && i < command.Length && command[i] != ' ')
            {
                if (command[i] == '"')
                    inQuotes = !inQuotes;

                i++;
            }

            if (i < command.Length)
                return new CommandParams
                {
                    Command = command.Substring(0, i),
                    Params = command.Substring(i)
                };
            else
                return new CommandParams
                {
                    Command = command,
                    Params = null
                };
        }

        private void Execute(string expression, ExecuteOptions options)
        {
            expression = expression.Trim();

            CommandParams enteredCommand = CommandBuilder.SplitCommand(expression);

            CustomCommand command = configuration.Commands
                .Where(c => c.Key.ToUpper() == enteredCommand.Command.ToUpper())
                .FirstOrDefault();

            if (command != null)
            {
                try
                {
                    List<string> parameters = CommandBuilder.BuildParameters(enteredCommand.Params);
                    string cmd = CommandBuilder.ApplyParameters(command.Command, parameters, enteredCommand.Params);

                    switch (command.CommandKind)
                    {
                        case CommandKinds.Command:
                            {
                                CommandParams cp = GetCommandInfo(cmd);
                                Process.Start(cp.Command, cp.Params);
                                break;
                            }
                        case CommandKinds.Url:
                            {
                                Process.Start(cmd);
                                break;
                            }
                        default:
                            throw new InvalidOperationException("Not supported command kind!");
                    }
                }
                catch (Exception e)
                {
                    options.ErrorText = $"Cannot execute: {e.Message}";
                    options.PreventClose = true;
                }
            }

        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            Func<CustomCommand, bool> func;

            CommandParams enteredCommand = CommandBuilder.SplitCommand(enteredText);

            if (perfectMatchesOnly)
                func = (CustomCommand command) => command.Key.ToUpper() == enteredCommand.Command.ToUpper() || command.Comment.ToUpper() == enteredCommand.Command.ToUpper();
            else
                func = (CustomCommand command) => command.Key.ToUpper().Contains(enteredCommand.Command.ToUpper()) || command.Comment.ToUpper().Contains(enteredCommand.Command.ToUpper());

            configuration.Commands
                .Where(func)
                .Select(c => new SuggestionInfo(c.Key + (enteredCommand.Params != null ? $" {enteredCommand.Params}" : string.Empty), 
                    c.Key, 
                    c.Comment, 
                    icon, 
                    SuggestionUtils.EvalMatch(enteredText, c.Key, c.Comment),
                    c))
                .ToList()
                .ForEach(c => collector.AddSuggestion(c));
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            Execute(expression, options);
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            Execute(suggestion.Text, options);
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            return null;
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
            icon = new BitmapImage(new Uri("pack://application:,,,/CustomCommandsModule;component/Resources/app.png"));
            configuration = new Configuration();
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

        public string Name
        {
            get
            {
                return MODULE_NAME;
            }
        }
    }
}
