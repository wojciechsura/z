using CustomCommandsModule.Common;
using CustomCommandsModule.Models;
using CustomCommandsModule.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CustomCommandsModule.ViewModels
{
    public class ConfigurationWindowViewModel : ICustomCommandCallbacks
    {
        private Configuration configuration;
        private IConfigurationWindowAccess access;

        private ObservableCollection<CustomCommandViewModel> customCommands;
        private readonly CommandKinds[] commandKinds;

        private void HandleAdd()
        {
            customCommands.Add(new CustomCommandViewModel(this));
        }

        private void HandleOK()
        {
            access.CloseWindow();
        }

        void ICustomCommandCallbacks.Delete(CustomCommandViewModel command)
        {
            customCommands.Remove(command);
        }

        public ConfigurationWindowViewModel(Configuration configuration)
        {
            this.configuration = configuration;

            customCommands = new ObservableCollection<CustomCommandViewModel>(configuration.Commands
                .Select(c => new CustomCommandViewModel(this, c)));

            AddCommand = new SimpleCommand((obj) => HandleAdd());
            OKCommand = new SimpleCommand((obj) => HandleOK());

            commandKinds = (CommandKinds[])Enum.GetValues(typeof(CommandKinds));
        }

        internal IEnumerable<string> Validate()
        {
            var errors = new List<string>();

            if (customCommands
                .Select(c => c.Key.ToUpper())
                .GroupBy(k => k)
                .Where(g => g.Count() > 1)
                .Any())
                errors.Add("Custom command keys must be unique!");

            if (customCommands
                .Any(c => c.Key.Contains(" ")))
                errors.Add("Custom command keys cannot contain space characters!");

            return errors;
        }

        public void Save()
        {
            configuration.Commands.Clear();

            customCommands
                .Select(cc => new CustomCommand
                {
                    Key = cc.Key,
                    Command = cc.Command,
                    CommandKind = cc.CommandKind,
                    Comment = cc.Comment
                })
                .ToList()
                .ForEach(c => configuration.Commands.Add(c));
        }

        public ObservableCollection<CustomCommandViewModel> CustomCommands => customCommands;

        public ICommand AddCommand { get; private set; }

        public ICommand OKCommand { get; private set; }

        public CommandKinds[] CommandKinds => commandKinds;

        public IConfigurationWindowAccess Access
        {
            set
            {
                access = value;
            }
        }
    }
}
