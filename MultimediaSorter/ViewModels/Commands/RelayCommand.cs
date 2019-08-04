using System;
using System.Windows.Input;

namespace MultimediaSorter.ViewModels.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Predicate<object> _predicate;

        public RelayCommand(Action<object> action, Predicate<object> predicate = null)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action), "Action can't be null");
            _predicate = predicate;
        }

        public bool CanExecute(object parameter = null)
        {
            return _predicate == null || _predicate(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter = null)
        {
            _action(parameter);
        }
    }
}
