using CustomCommandsModule.Common;
using CustomCommandsModule.Models;
using CustomCommandsModule.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CustomCommandsModule.ViewModels
{
    public class CustomCommandViewModel
    {
        private readonly ICustomCommandCallbacks callbacks;

        public string Key { get; set; }
        public string Comment { get; set; }
        public string Command { get; set; }
        public CommandKinds CommandKind { get; set; }

        private void HandleDelete()
        {
            callbacks.Delete(this);
        }

        private void Init()
        {
            DeleteCommand = new SimpleCommand((obj) => HandleDelete());
        }

        public CustomCommandViewModel(ICustomCommandCallbacks callbacks)
        {
            this.callbacks = callbacks;

            CommandKind = CommandKinds.Command;

            Init();
        }

        public CustomCommandViewModel(ICustomCommandCallbacks callbacks, CustomCommand sourceCommand)
        {
            this.callbacks = callbacks;

            Key = sourceCommand.Key;
            Comment = sourceCommand.Comment;
            Command = sourceCommand.Command;
            CommandKind = sourceCommand.CommandKind;

            Init();
        }

        public ICommand DeleteCommand { get; private set; }
    }
}
