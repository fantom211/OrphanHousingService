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
using System.Configuration;
using System.Data;
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
        protected override void OnStartup(StartupEventArgs e)
        {
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
            vm.Initialize();
            base.OnStartup(e);
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
            services.AddTransient<ApartmentsViewModel>();
            services.AddTransient<PeopleViewModel>();
            services.AddTransient<ContractsViewModel>();
            services.AddTransient<AddPersonViewModel>();
            services.AddTransient<AddApartmentViewModel>();
            services.AddTransient<AddApartmentStatusHistoryViewModel>();
            services.AddTransient<ApartmentStatusHistoriesViewModel>();
            services.AddTransient<AddContractViewModel>();
            services.AddTransient<ApplicationsViewModel>();
            services.AddTransient<AddApplicationViewModel>();
            services.AddTransient<ApartmentStatusHistoryDetailsViewModel>();
            services.AddTransient<CommissionDecisionsViewModel>();
            services.AddTransient<AddCommissionDecisionViewModel>();
            services.AddTransient<PersonDetailsViewModel>();
            services.AddTransient<ApartmentDetailsViewModel>();
            services.AddTransient<ContractHistoryViewModel>();
            services.AddTransient<ContractDetailsViewModel>();
            services.AddTransient<ApplicationDetailsViewModel>();
            services.AddTransient<CommissionDecisionDetailsViewModel>();
            services.AddTransient<UtilityDebtDetailsViewModel>();
            services.AddTransient<FamilyMemberDetailsViewModel>();
            services.AddTransient<UtilityDebtsViewModel>();
            services.AddTransient<FamilyMembersViewModel>();
            services.AddTransient<AddUtilityDebtViewModel>();
            services.AddTransient<AddFamilyMemberViewModel>();

            //Сервисы
            services.AddTransient<ContractService>();
            services.AddTransient<ApartmentService>();
            services.AddTransient<ApartmentStatusHistoryService>();
            services.AddTransient<PersonService>();
            services.AddTransient<UtilityDebtService>();
            services.AddTransient<FamilyMemberService>();
            services.AddTransient<ApplicationService>();
            services.AddTransient<CommissionDecisionService>();
            services.AddTransient<ContractWorkFlowService>();
            services.AddTransient<ContractHistoryService>();
            services.AddTransient(typeof(CrudService<>));
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
