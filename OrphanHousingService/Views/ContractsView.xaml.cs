using OrphanHousingService.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OrphanHousingService.Views
{
    public partial class ContractsView : UserControl
    {
        public ContractsView()
        {
            InitializeComponent();
        }

        private void ContractsDataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = FindParent<DataGridRow>(e.OriginalSource as DependencyObject);
            if (row == null)
                return;

            ContractsDataGrid.SelectedItem = row.DataContext;

            if (row.DataContext is Contract contract)
            {
                ContractsDataGrid.CurrentCell = new DataGridCellInfo(contract, ContractsDataGrid.Columns[0]);
                ContractsDataGrid.ScrollIntoView(contract);
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
