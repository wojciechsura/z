using ControlPanelModule.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Z.Api.Interfaces;
using Z.Api.Types;

namespace ControlPanelModule
{
    public class Module : IZModule
    {
        private const string MODULE_NAME = "ControlPanel";
        private const string MODULE_DISPLAY_NAME = "Control panel";

        private List<BaseControlPanelEntry> controlPanelEntries;
        private BitmapImage icon;

        private class CommandInfo
        {
            public string Command { get; set; }
            public string Parameters { get; set; }
        }

        private CommandInfo GetCommandInfo(string command)
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
                return new CommandInfo
                {
                    Command = command.Substring(0, i),
                    Parameters = command.Substring(i)
                };
            else
                return new CommandInfo
                {
                    Command = command,
                    Parameters = null
                };
        }

        private void RunEntry(BaseControlPanelEntry entry)
        {
            if (entry is CommandControlPanelEntry)
            {
                CommandInfo info = GetCommandInfo((entry as CommandControlPanelEntry).Command);

                Process.Start(info.Command, info.Parameters);
            }
            else if (entry is ShellFolderControlPanelEntry)
            {
                Process.Start("explorer", $"shell:::{entry.Namespace}");
            }
            else
                throw new ArgumentException("Invalid entry type!");
        }

        public Module()
        {
            controlPanelEntries = ControlPanelItemProvider.GetEntries()
                .OrderBy(e => e.DisplayName)
                .ToList();

            icon = new BitmapImage(new Uri("pack://application:,,,/ControlPanelModule;component/Resources/cpanel.png"));
        }

        public void CollectSuggestions(string enteredText, string keywordAction, ISuggestionCollector collector)
        {
            controlPanelEntries
                .Where(cpe => cpe.DisplayName.ToUpper().Contains(enteredText.ToUpper()))
                .OrderBy(cpe => cpe.DisplayName)
                .Select(cpe => new SuggestionInfo(cpe.DisplayName, cpe.DisplayName, cpe.InfoTip, icon, cpe))
                .ToList()
                .ForEach(s => collector.AddSuggestion(s));
        }

        public void ExecuteKeywordAction(string action, string expression)
        {
            BaseControlPanelEntry entry = controlPanelEntries.FirstOrDefault(cpe => cpe.DisplayName.ToUpper() == expression.ToUpper());
            if (entry != null)
                RunEntry(entry);
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion)
        {
            BaseControlPanelEntry entry = suggestion.Data as BaseControlPanelEntry;
            if (entry != null)
                RunEntry(entry);
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo("cpanel", "ControlPanelEntry", "Control panel");
        }

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
