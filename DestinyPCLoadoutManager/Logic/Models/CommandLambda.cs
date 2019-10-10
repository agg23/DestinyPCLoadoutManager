using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DestinyPCLoadoutManager.Logic.Models
{
    class CommandLambda: ICommand
    {
        Action func;
        
        public CommandLambda(Action func)
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
            func();
        }
    }
}
