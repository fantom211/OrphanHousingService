using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace OrphanHousingService.Behaviors
{
    public class DataGridScrollIntoViewBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
            base.OnDetaching();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociatedObject.SelectedItem != null)
                AssociatedObject.ScrollIntoView(AssociatedObject.SelectedItem);
        }
    }
}
