using OrphanHousingService.ViewModels.CrudViewModels;
using System.Windows;

namespace OrphanHousingService.Views.CrudViews
{
    public partial class AddFamilyMemberView : Window
    {
        public AddFamilyMemberView(AddFamilyMemberViewModel vm)
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
