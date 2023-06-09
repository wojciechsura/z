﻿using ControlPanelModule.Infrastructure;
using ControlPanelModule.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.Api;
using Z.Api.Interfaces;
using Z.Api.Types;
using Z.Api.Utils;

namespace ControlPanelModule
{
    public class Module : IZModule
    {
        private const string MODULE_NAME = "ControlPanel";
        private const string CPANEL_KEYWORD = "cpanel";
        private const string CPANEL_KEYWORD_ACTION = "ControlPanelEntry";
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

        public void CollectSuggestions(string enteredText, string action, bool perfectMatchesOnly, ISuggestionCollector collector)
        {
            Func<BaseControlPanelEntry, bool> filter;
            if (perfectMatchesOnly)
                filter = cpe => cpe.DisplayName.ToUpper() == enteredText.ToUpper();
            else
                filter = cpe => cpe.DisplayName.ToUpper().Contains(enteredText.ToUpper());

            controlPanelEntries
                .Where(filter)
                .OrderBy(cpe => cpe.DisplayName)
                .Select(cpe => new SuggestionInfo(cpe.DisplayName, cpe.DisplayName, cpe.InfoTip, icon, SuggestionUtils.EvalMatch(enteredText, cpe.DisplayName), cpe))
                .ToList()
                .ForEach(s => collector.AddSuggestion(s));
        }

        public void ExecuteKeywordAction(string action, string expression, ExecuteOptions options)
        {
            BaseControlPanelEntry entry = controlPanelEntries.FirstOrDefault(cpe => cpe.DisplayName.ToUpper() == expression.ToUpper());
            if (entry != null)
                RunEntry(entry);
        }

        public void ExecuteSuggestion(SuggestionInfo suggestion, ExecuteOptions options)
        {
            BaseControlPanelEntry entry = suggestion.Data as BaseControlPanelEntry;
            if (entry != null)
                RunEntry(entry);
        }

        public IEnumerable<KeywordInfo> GetKeywordActions()
        {
            yield return new KeywordInfo(CPANEL_KEYWORD, CPANEL_KEYWORD_ACTION, Strings.ControlPanel_KeywordDisplayName, Strings.ControlPanel_KeywordComment);
        }

        public string DisplayName
        {
            get
            {
                return Strings.ControlPanel_ModuleDisplayName;
            }
        }

        public string Name
        {
            get
            {
                return MODULE_NAME;
            }
        }

        public ImageSource Icon => icon;
    }
}
