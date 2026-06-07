using OrphanHousingService.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OrphanHousingService.Views
{
    public partial class PeopleView : UserControl
    {
        public PeopleView()
        {
            InitializeComponent();
        }

        private void PeopleDataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = FindParent<DataGridRow>(e.OriginalSource as DependencyObject);
            if (row == null)
                return;

            PeopleDataGrid.SelectedItem = row.DataContext;

            if (row.DataContext is Person person)
            {
                PeopleDataGrid.CurrentCell = new DataGridCellInfo(person, PeopleDataGrid.Columns[0]);
                PeopleDataGrid.ScrollIntoView(person);
            }
        }

        private static T? FindParent<T>(DependencyObject? child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                    return parent;

                child = VisualTreeHelper.GetParent(child);
            }

            return null;
        }
    }
}
