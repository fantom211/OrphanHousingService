using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using OrphanHousingService.Models.Reports;

using OrphanHousingService.Services.Business;

using OrphanHousingService.ViewModels.Helpers;

using OrphanHousingService.ViewModels.Interfaces;

using System.Windows;



namespace OrphanHousingService.ViewModels

{

    public partial class ReportsViewModel : ObservableObject, ISearchableListViewModel

    {

        private readonly ReportService _reportService;

        private readonly WordReportExportService _wordExportService;



        [ObservableProperty]

        private ReportModel? currentReport;



        [ObservableProperty]

        private string? notes;



        [ObservableProperty]

        private string? searchText;



        [ObservableProperty]

        private bool usePeriodFilter;



        [ObservableProperty]

        private DateTime? periodFrom;



        [ObservableProperty]

        private DateTime? periodTo;



        public ReportsViewModel(

            ReportService reportService,

            WordReportExportService wordExportService)

        {

            _reportService = reportService;

            _wordExportService = wordExportService;

            _ = ViewModelLoadHelper.RunSafeAsync(GenerateReportAsync, "Отчёты");

        }



        partial void OnSearchTextChanged(string? value) { }



        [RelayCommand]

        private async Task GenerateReport()

        {

            await GenerateReportAsync();

        }



        private async Task GenerateReportAsync()

        {

            if (!TryGetPeriodBounds(out var from, out var to))

                return;



            CurrentReport = await _reportService.BuildSummaryReportAsync(Notes, from, to);

        }



        private bool TryGetPeriodBounds(out DateTime? from, out DateTime? to)

        {

            from = null;

            to = null;



            if (!UsePeriodFilter)

                return true;



            from = PeriodFrom?.Date;

            to = PeriodTo?.Date;



            if (!from.HasValue && !to.HasValue)

            {

                ValidationDialogHelper.ShowError(

                    new InvalidOperationException("Укажите дату начала и/или окончания периода."));

                return false;

            }



            if (from.HasValue && to.HasValue && from.Value > to.Value)

            {

                ValidationDialogHelper.ShowError(

                    new InvalidOperationException("Дата начала не может быть позже даты окончания."));

                return false;

            }



            return true;

        }



        [RelayCommand(CanExecute = nameof(CanExport))]

        private void ExportToWord()

        {

            if (CurrentReport == null)

                return;



            var dialog = new SaveFileDialog

            {

                Filter = "Word документ (*.docx)|*.docx",

                FileName = $"Отчёт_{DateTime.Now:yyyyMMdd_HHmm}.docx",

                DefaultExt = ".docx"

            };



            if (dialog.ShowDialog() != true)

                return;



            try

            {

                _wordExportService.ExportToWord(CurrentReport, dialog.FileName);

                MessageBox.Show(

                    $"Отчёт сохранён:\n{dialog.FileName}",

                    "Экспорт",

                    MessageBoxButton.OK,

                    MessageBoxImage.Information);

            }

            catch (Exception ex)

            {

                ValidationDialogHelper.ShowError(ex);

            }

        }



        private bool CanExport() => CurrentReport != null;



        partial void OnCurrentReportChanged(ReportModel? value) =>

            ExportToWordCommand.NotifyCanExecuteChanged();



        IRelayCommand ICrudViewModel.AddCommand => DisabledCommand.Instance;

        IRelayCommand ICrudViewModel.EditCommand => DisabledCommand.Instance;

        IRelayCommand ICrudViewModel.DeleteCommand => DisabledCommand.Instance;



        private sealed class DisabledCommand : IRelayCommand

        {

            public static readonly DisabledCommand Instance = new();

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter) => false;

            public void Execute(object? parameter) { }

            public void NotifyCanExecuteChanged() { }

        }

    }

}


