using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OrphanHousingService.Behaviors
{
    public class DataGridRowRightClickBehavior : Behavior<DataGridRow>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand),
                typeof(DataGridRowRightClickBehavior));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseRightButtonUp += OnRightClick;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseRightButtonUp -= OnRightClick;
        }

        private void OnRightClick(object sender, MouseButtonEventArgs e)
        {
            var row = AssociatedObject;
            var contract = row.DataContext;

            if (Command?.CanExecute(contract) == true)
                Command.Execute(contract);
        }
    }
}
