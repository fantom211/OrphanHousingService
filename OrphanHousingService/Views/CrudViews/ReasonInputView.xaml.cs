using OrphanHousingService.ViewModels.CrudViewModels;
using System.Windows;

namespace OrphanHousingService.Views.CrudViews
{
    public partial class ReasonInputView : Window
    {
        public ReasonInputView(ReasonInputViewModel vm)
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
