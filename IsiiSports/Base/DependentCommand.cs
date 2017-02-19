using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Xamarin.Forms;

namespace IsiiSports.Base
{
    public class DependentCommand : Command
    {
        private readonly List<string> dependentPropertyNames;

        public DependentCommand(Action execute, Func<bool> canExecute, INotifyPropertyChanged target, params string[] dependentPropertyNames)
      : base(execute, canExecute)
        {
            this.dependentPropertyNames = new List<string>(dependentPropertyNames);
            target.PropertyChanged += TargetPropertyChanged;
        }

        public DependentCommand(Action execute, Func<bool> canExecute, INotifyPropertyChanged target, params Expression<Func<object>>[] dependentPropertyExpressions)
      : base(execute, canExecute)
        {
            dependentPropertyNames = new List<string>();
            foreach (var expression in dependentPropertyExpressions.Select(expression => expression.Body))
            {
                var memberExpression = expression as MemberExpression;
                if (memberExpression != null)
                {
                    dependentPropertyNames.Add(memberExpression.Member.Name);
                }
                else
                {
                    var unaryExpression = expression as UnaryExpression;
                    if (unaryExpression != null)
                        dependentPropertyNames.Add(((MemberExpression)unaryExpression.Operand).Member.Name);
                }
            }
            target.PropertyChanged += TargetPropertyChanged;
        }

        private void TargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!dependentPropertyNames.Contains(e.PropertyName))
                return;
            ChangeCanExecute();
        }
    }
}
