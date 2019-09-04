using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WorkUwpApp.Helpers
{
    public sealed class DelegateCommand : ICommand
    {
        private SimpleEventHandler handler;
        private bool isEnabled = true;
        public event EventHandler CanExecuteChanged;
        public delegate void SimpleEventHandler();
        
        public DelegateCommand(SimpleEventHandler handler)
        {
            this.handler = handler;
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
        }

        void ICommand.Execute(object org)
        {
            this.handler();
        }
        bool ICommand.CanExecute(object org)
        {
            return this.IsEnabled;
        }
        private void OnCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
