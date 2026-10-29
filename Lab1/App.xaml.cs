using System.Windows;
using TravelAgency.DataAccess;
using TravelAgency.DataAccess.Repositories;
using TravelAgency.Reports;
using TravelAgency.WPF.ViewModels;
using TravelAgency.WPF.Views;

namespace TravelAgency.WPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Строка подключения к MSSQL LocalDB / SQL Express
        const string connString = "Server=(localdb)\\MSSQLLocalDB;Database=TravelAgencyDB;Trusted_Connection=True;TrustServerCertificate=True;";

        var dbFactory = new DbConnectionFactory(connString);
        var tourRepo = new TourRepository(dbFactory);
        var empRepo = new EmployeeRepository(dbFactory);
        var contractRepo = new ContractRepository(dbFactory);
        var queryRepo = new QueryRepository(dbFactory);
        var reportService = new WordReportService();

        var mainVm = new MainViewModel(tourRepo, empRepo, contractRepo, queryRepo, reportService);

        var mainWindow = new MainWindow { DataContext = mainVm };
        this.MainWindow = mainWindow;
        mainWindow.Show();
    }
}