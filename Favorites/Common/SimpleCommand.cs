using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FavoritesModule.Common
{
    class SimpleCommand : ICommand
    {
        private Action<object> action;
        private Func<object, bool> canExecute;

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public SimpleCommand(Action<object> action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            this.canExecute = (o) => true;
        }

        public SimpleCommand(Action<object> action, Func<object, bool> canExecute)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

        public void NotifyCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        public event EventHandler CanExecuteChanged;
    }
}
