using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Repository;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using OrphanHousingService.Services.Validators;
using OrphanHousingService.ViewModels;
using OrphanHousingService.ViewModels.CrudViewModels;
using OrphanHousingService.ViewModels.Details;
using OrphanHousingService.Views;
using OrphanHousingService.Views.CrudViews;
using OrphanHousingService.Views.Details;
using OrphanHousingService.Resources;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ApplicationModel = OrphanHousingService.Models.Application;

namespace OrphanHousingService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider Services { get; private set; }
        public static IConfiguration Configuration { get; private set; }
        protected override async void OnStartup(StartupEventArgs e)
        {
            var culture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Enums.Culture = culture;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            var services = new ServiceCollection();

            ConfigureServices(services);

            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            var vm = Services.GetRequiredService<MainViewModel>();
            mainWindow.DataContext = vm;
            mainWindow.Show();

            await Task.Run(RunDatabaseStartupInternal).ConfigureAwait(true);

            vm.Initialize();
            base.OnStartup(e);
        }

        private static void RunDatabaseStartupInternal()
        {
            try
            {
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrphanHousingDbContext>();
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                Current.Dispatcher.Invoke(() => MessageBox.Show(
                    $"Не удалось применить миграции базы данных.\n{ex.Message}\n\n" +
                    "Данные могут не отображаться, пока схема БД не соответствует модели приложения.",
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error));
            }

            try
            {
                using var scope = Services.CreateScope();
                var systemEntityService = scope.ServiceProvider.GetRequiredService<SystemEntityService>();
                systemEntityService.EnsureSeededAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Current.Dispatcher.Invoke(() => MessageBox.Show(
                    $"Не удалось инициализировать системные сущности.\n{ex.Message}",
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error));
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OrphanHousingDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            //Окна
            services.AddSingleton<MainWindow>();
            services.AddTransient<ContractsView>();
            services.AddTransient<ApartmentsView>();
            services.AddTransient<ApartmentStatusHistoriesView>();
            services.AddTransient<PeopleView>();
            services.AddTransient<AddPersonView>();
            services.AddTransient<AddApartmentView>();
            services.AddTransient<AddApartmentStatusHistoryView>();
            services.AddTransient<AddContractView>();
            services.AddTransient<ApplicationsView>();
            services.AddTransient<AddApplicationView>();
            services.AddTransient<ApartmentStatusHistoryDetailsView>();
            services.AddTransient<CommissionDecisionsView>();
            services.AddTransient<AddCommissionDecisionView>();
            services.AddTransient<PersonDetailsView>();
            services.AddTransient<ApartmentDetailsView>();
            services.AddTransient<ContractHistoryView>();
            services.AddTransient<ContractDetailsView>();
            services.AddTransient<ApplicationDetailsView>();
            services.AddTransient<CommissionDecisionDetailsView>();
            services.AddTransient<UtilityDebtDetailsView>();
            services.AddTransient<FamilyMemberDetailsView>();
            services.AddTransient<UtilityDebtsView>();
            services.AddTransient<FamilyMembersView>();
            services.AddTransient<AddUtilityDebtView>();
            services.AddTransient<AddFamilyMemberView>();

            //ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddScoped<ApartmentsViewModel>();
            services.AddScoped<PeopleViewModel>();
            services.AddScoped<ContractsViewModel>();
            services.AddScoped<AddPersonViewModel>();
            services.AddScoped<AddApartmentViewModel>();
            services.AddScoped<AddApartmentStatusHistoryViewModel>();
            services.AddScoped<ApartmentStatusHistoriesViewModel>();
            services.AddScoped<AddContractViewModel>();
            services.AddScoped<ApplicationsViewModel>();
            services.AddScoped<AddApplicationViewModel>();
            services.AddScoped<ApartmentStatusHistoryDetailsViewModel>();
            services.AddScoped<CommissionDecisionsViewModel>();
            services.AddScoped<AddCommissionDecisionViewModel>();
            services.AddScoped<PersonDetailsViewModel>();
            services.AddScoped<ApartmentDetailsViewModel>();
            services.AddScoped<ContractHistoryViewModel>();
            services.AddScoped<ContractDetailsViewModel>();
            services.AddScoped<ApplicationDetailsViewModel>();
            services.AddScoped<CommissionDecisionDetailsViewModel>();
            services.AddScoped<UtilityDebtDetailsViewModel>();
            services.AddScoped<FamilyMemberDetailsViewModel>();
            services.AddScoped<UtilityDebtsViewModel>();
            services.AddScoped<FamilyMembersViewModel>();
            services.AddScoped<AddUtilityDebtViewModel>();
            services.AddScoped<AddFamilyMemberViewModel>();

            //Сервисы
            services.AddScoped<ContractService>();
            services.AddScoped<ApartmentService>();
            services.AddScoped<ApartmentStatusHistoryService>();
            services.AddScoped<PersonService>();
            services.AddScoped<UtilityDebtService>();
            services.AddScoped<FamilyMemberService>();
            services.AddScoped<ApplicationService>();
            services.AddScoped<CommissionDecisionService>();
            services.AddScoped<ContractWorkFlowService>();
            services.AddScoped<ContractHistoryService>();
            services.AddScoped<SystemEntityService>();
            services.AddScoped(typeof(CrudService<>));
            services.AddScoped<IValidator<Contract>, ContractValidator>();
            services.AddScoped<IValidator<Apartment>, ApartmentValidator>();
            services.AddScoped<IValidator<ApplicationModel>, ApplicationValidator>();
            services.AddScoped<IValidator<ApartmentStatusHistory>, ApartmentStatusHistoryValidator>();
            services.AddScoped<IValidator<CommissionDecision>, CommissionDecisionValidator>();
            services.AddScoped<IValidator<FamilyMember>, FamilyMemberValidator>();
            services.AddScoped<IValidator<Person>, PersonValidator>();
            services.AddScoped<IValidator<UtilityDebt>, UtilityDebtValidator>();


        }
    }
}
