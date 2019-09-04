using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WorkUwpApp.Helpers
{
    public class ComplexCommand : ICommand 
    {
        private readonly Action<object> _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public ComplexCommand(Action<object> execute) : this(execute, null)
        {
        }

        public ComplexCommand(Action<object> execute, Func<bool> canExecute)
        {
            this._execute = execute ?? throw new ArgumentException("execute");
            this._canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => this._canExecute == null ? true : this._canExecute();

        public void Execute(object parameter) => this._execute(parameter);

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public void RaiseCanExecuteChanged(string propertyName)
        {
            this.CanExecuteChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}