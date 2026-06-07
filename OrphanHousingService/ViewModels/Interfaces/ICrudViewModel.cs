using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels.Interfaces
{
    public interface ICrudViewModel
    {
        IRelayCommand AddCommand { get; }
        IRelayCommand EditCommand { get; }
        IRelayCommand DeleteCommand { get; }
    }
}
