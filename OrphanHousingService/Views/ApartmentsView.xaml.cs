using OrphanHousingService.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OrphanHousingService.Views
{
    public partial class ApartmentsView : UserControl
    {
        public ApartmentsView()
        {
            InitializeComponent();
        }

        private void ApartmentsDataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = FindParent<DataGridRow>(e.OriginalSource as DependencyObject);
            if (row == null)
                return;

            ApartmentsDataGrid.SelectedItem = row.DataContext;

            if (row.DataContext is Apartment apartment)
            {
                ApartmentsDataGrid.CurrentCell = new DataGridCellInfo(apartment, ApartmentsDataGrid.Columns[0]);
                ApartmentsDataGrid.ScrollIntoView(apartment);
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
