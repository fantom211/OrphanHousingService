using OrphanHousingService.ViewModels.CrudViewModels;
using System.Windows;

namespace OrphanHousingService.Views.CrudViews
{
    public partial class AddUtilityDebtView : Window
    {
        public AddUtilityDebtView(AddUtilityDebtViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;

            vm.CloseAction = result =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}
