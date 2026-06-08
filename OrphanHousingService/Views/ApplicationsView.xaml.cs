using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ApplicationModel = OrphanHousingService.Models.Application;

namespace OrphanHousingService.Views
{
    public partial class ApplicationsView : UserControl
    {
        public ApplicationsView()
        {
            InitializeComponent();
        }

        private void ApplicationsDataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = FindParent<DataGridRow>(e.OriginalSource as DependencyObject);
            if (row == null)
                return;

            ApplicationsDataGrid.SelectedItem = row.DataContext;

            if (row.DataContext is ApplicationModel application)
            {
                ApplicationsDataGrid.CurrentCell = new DataGridCellInfo(application, ApplicationsDataGrid.Columns[0]);
                ApplicationsDataGrid.ScrollIntoView(application);
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
