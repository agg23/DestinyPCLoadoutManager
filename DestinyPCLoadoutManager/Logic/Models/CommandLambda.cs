using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DestinyPCLoadoutManager.Logic.Models
{
    class CommandLambda<T>: ICommand
    {
        Action<T> func;
        
        public CommandLambda(Action<T> func)
        {
            this.func = func;
        }
        
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            func((T)parameter);
        }
    }
}
