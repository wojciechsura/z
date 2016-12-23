using CustomCommandsModule.Infrastructure;
using CustomCommandsModule.Models;
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

namespace CustomCommandsModule
{
    public class Module : IZModule, IZInitializable
    {
        private class CommandParams
        {
            public string Command { get; set; }
            public string Params { get; set; }
        }

        private const string MODULE_DISPLAY_NAME = "Custom";
        private const string MODULE_NAME = "Custom";
        private const string CONFIG_FILENAME = "config.xml";
        private readonly ImageSource icon;
        private Configuration configuration;
        private IModuleContext context;

        private CommandParams ExtractCommand(string expression)
        {
            expression = expression.TrimStart();

            int pos = expression.IndexOf(' ');
            if (pos == -1)
                return new CommandParams { Command = expression, Params = null };
            else
                return new CommandParams { Command = expression.Substring(0, pos), Params = pos + 1 > expression.Length ? string.Empty : expression.Substring(pos + 1) };
        }

        private List<string> BuildParameters(string parameters)
        {
            List<string> result = new List<string>();
            ParameterParser parser = new ParameterParser();

            int i = 0;
            while (i < parameters.Length)
            {
                ParameterToken token = parser.Process(parameters, ref i);

                if (token.TokenType == ParameterTokenType.QuotedParameter)
                {
                    StringBuilder builder = new StringBuilder();
                    string baseParam = token.TokenStr.Substring(1, token.TokenStr.Length - 2);

                    int j = 0;
                    while (j < baseParam.Length)
                    {
                        if (baseParam[j] == '"')
                        {
                            j++;
                            if (j >= baseParam.Length || baseParam[j] != '"')
                                throw new InvalidOperationException("Parser algorithm broken!");

                            builder.Append('"');
                            j++;
                        }
                        else
                        {
                            builder.Append(baseParam[j]);
                            j++;
                        }
                    }

                    result.Add(builder.ToString());
                }
                else if (token.TokenType == ParameterTokenType.RegularParameter)
                {
                    result.Add(token.TokenStr);
                }
                else if (token.TokenType == ParameterTokenType.Unknown)
                    throw new InvalidOperationException("Broken parameters!");
            }

            return result;
        }
         
        private string ApplyParameters(string text, List<string> parameters, string parameterString)
        {
            CommandParser parser = new CommandParser();
            int i = 0;
            StringBuilder builder = new StringBuilder();

            while (i < text.Length)
            {
                CommandToken token = parser.Process(text, ref i);
                switch (token.TokenType)
                {
                    case CommandTokenType.RegularText:
                        {
                            builder.Append(token.TokenStr);
                            break;
                        }
                    case CommandTokenType.OpeningBrace:
                        {
                            builder.Append('{');
                            break;
                        }
                    case CommandTokenType.ClosingBrace:
                        {
                            builder.Append('}');
                            break;
                        }
                    case CommandTokenType.Parameter:
                        {
                            int parameterId = int.Parse(token.TokenStr.Substring(1, token.TokenStr.Length - 2));
                            if (parameterId < parameters.Count)
                                builder.Append(parameters[parameterId]);
                            break;
                        }
                    case CommandTokenType.UrlParameter:
                        {
                            int parameterId = int.Parse(token.TokenStr.Substring(2, token.TokenStr.Length - 3));
                            if (parameterId < parameters.Count)
                                builder.Append(WebUtility.UrlEncode(parameters[parameterId]));
                            break;
                        }
                    case CommandTokenType.All:
                        {
                            builder.Append(parameterString);
                            break;
                        }
                    case CommandTokenType.UrlAll:
                        {
                            builder.Append(WebUtility.UrlEncode(parameterString));
                            break;
                        }
                    case CommandTokenType.Unknown:
                        throw new InvalidOperationException("Invalid command!");
                }
            }

            return builder.ToString();
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

        private void Execute(string expression)
        {
            expression = expression.Trim();

            CommandParams enteredCommand = ExtractCommand(expression);

            CustomCommand command = configuration.Commands
                .Where(c => c.Key.ToUpper() == enteredCommand.Command.ToUpper())
                .FirstOrDefault();

            if (command != null)
            {
                try
                {
                    List<string> parameters = BuildParameters(enteredCommand.Params);
                    string cmd = ApplyParameters(command.Command, parameters, enteredCommand.Params);

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
                catch
                {
                    // TODO notify user
                }
            }

        }

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            Func<CustomCommand, bool> func;

            CommandParams enteredCommand = ExtractCommand(enteredText);

            if (perfectMatchesOnly)
                func = (CustomCommand command) => command.Key.ToUpper() == enteredCommand.Command.ToUpper();
            else
                func = (CustomCommand command) => command.Key.ToUpper().Contains(enteredCommand.Command.ToUpper());

            configuration.Commands
                .Where(func)
                .Select(c => new SuggestionInfo(c.Key + (enteredCommand.Params != null ? $" {enteredCommand.Params}" : string.Empty), c.Key, c.Comment, icon, c))
                .ToList()
                .ForEach(c => collector.AddSuggestion(c));
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            Execute(expression);
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            Execute(suggestion.Text);
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
