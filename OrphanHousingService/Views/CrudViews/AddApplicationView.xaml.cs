using OrphanHousingService.ViewModels.CrudViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OrphanHousingService.Views.CrudViews
{
    /// <summary>
    /// Логика взаимодействия для AddApplicationView.xaml
    /// </summary>
    public partial class AddApplicationView : Window
    {
        public AddApplicationView(AddApplicationViewModel vm)
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
