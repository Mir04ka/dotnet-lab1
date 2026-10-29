using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TravelAgency.Domain.DTOs;
using TravelAgency.Domain.Entities;
using TravelAgency.Domain.Interfaces;
using TravelAgency.Reports;
using TravelAgency.WPF.Commands;
using TravelAgency.WPF.Views;

namespace TravelAgency.WPF.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ITourRepository _tourRepo;
    private readonly IEmployeeRepository _empRepo;
    private readonly IContractRepository _contractRepo;
    private readonly IQueryRepository _queryRepo;
    private readonly WordReportService _reportService;

    // Списки данных
    public ObservableCollection<Tour> Tours { get; set; } = new();
    public ObservableCollection<Tour> FilteredTours { get; set; } = new();
    public ObservableCollection<Employee> Employees { get; set; } = new();
    public ObservableCollection<Contract> Contracts { get; set; } = new();

    // Результаты запросов
    public ObservableCollection<Contract> QueryDateContracts { get; set; } = new();
    public ObservableCollection<EmployeeTopDto> QueryTopEmployees { get; set; } = new();
    public ObservableCollection<Tour> QueryMinDurationTours { get; set; } = new();
    public ObservableCollection<CustomerTourDto> QueryCustomerTours { get; set; } = new();
    public ObservableCollection<EmployeeDailySalesDto> QueryGroupedSales { get; set; } = new();

    // Параметры фильтрации Туров
    private string _filterDestination = string.Empty;
    public string FilterDestination
    {
        get => _filterDestination;
        set { _filterDestination = value; OnPropertyChanged(); ApplyToursFilter(); }
    }

    private int _filterMinDays = 0;
    public int FilterMinDays
    {
        get => _filterMinDays;
        set { _filterMinDays = value; OnPropertyChanged(); ApplyToursFilter(); }
    }

    private int _filterHotelClass = 0; // 0 = Все
    public int FilterHotelClass
    {
        get => _filterHotelClass;
        set { _filterHotelClass = value; OnPropertyChanged(); ApplyToursFilter(); }
    }

    // Параметры для выполнения запросов
    public DateTime SelectedQueryDate { get; set; } = DateTime.Today;
    public int SelectedMinContracts { get; set; } = 2;
    public int SelectedMinDuration { get; set; } = 7;
    public string SelectedCustomerName { get; set; } = string.Empty;

    // Выбранные элементы
    public Tour? SelectedTour { get; set; }
    public Employee? SelectedEmployee { get; set; }
    public Contract? SelectedContract { get; set; }

    // Команды CRUD
    public ICommand AddTourCommand { get; }
    public ICommand EditTourCommand { get; }
    public ICommand DeleteTourCommand { get; }

    public ICommand AddEmployeeCommand { get; }
    public ICommand EditEmployeeCommand { get; }
    public ICommand DeleteEmployeeCommand { get; }

    public ICommand AddContractCommand { get; }
    public ICommand EditContractCommand { get; }
    public ICommand DeleteContractCommand { get; }

    // Команды Запросов
    public ICommand RunQuery1Command { get; }
    public ICommand RunQuery2Command { get; }
    public ICommand RunQuery3Command { get; }
    public ICommand RunQuery4Command { get; }
    public ICommand RunQuery5Command { get; }

    // Команды Отчетов в Word
    public ICommand ExportToursReportCommand { get; }
    public ICommand ExportCustomerReportCommand { get; }
    public ICommand ExportGroupedReportCommand { get; }

    public MainViewModel(
        ITourRepository tourRepo,
        IEmployeeRepository empRepo,
        IContractRepository contractRepo,
        IQueryRepository queryRepo,
        WordReportService reportService)
    {
        _tourRepo = tourRepo;
        _empRepo = empRepo;
        _contractRepo = contractRepo;
        _queryRepo = queryRepo;
        _reportService = reportService;

        // Инициализация команд CRUD
        AddTourCommand = new RelayCommand(async _ => await AddTour());
        EditTourCommand = new RelayCommand(async _ => await EditTour(), _ => SelectedTour != null);
        DeleteTourCommand = new RelayCommand(async _ => await DeleteTour(), _ => SelectedTour != null);

        AddEmployeeCommand = new RelayCommand(async _ => await AddEmployee());
        EditEmployeeCommand = new RelayCommand(async _ => await EditEmployee(), _ => SelectedEmployee != null);
        DeleteEmployeeCommand = new RelayCommand(async _ => await DeleteEmployee(), _ => SelectedEmployee != null);

        AddContractCommand = new RelayCommand(async _ => await AddContract());
        EditContractCommand = new RelayCommand(async _ => await EditContract(), _ => SelectedContract != null);
        DeleteContractCommand = new RelayCommand(async _ => await DeleteContract(), _ => SelectedContract != null);

        // Запросы
        RunQuery1Command = new RelayCommand(async _ => await LoadQuery1());
        RunQuery2Command = new RelayCommand(async _ => await LoadQuery2());
        RunQuery3Command = new RelayCommand(async _ => await LoadQuery3());
        RunQuery4Command = new RelayCommand(async _ => await LoadQuery4());
        RunQuery5Command = new RelayCommand(async _ => await LoadQuery5());

        // Отчеты
        ExportToursReportCommand = new RelayCommand(_ => GenerateToursReport());
        ExportCustomerReportCommand = new RelayCommand(_ => GenerateCustomerReport());
        ExportGroupedReportCommand = new RelayCommand(_ => GenerateGroupedReport());

        _ = LoadAllDataAsync();
    }

    public async Task LoadAllDataAsync()
    {
        try
        {
            var tours = await _tourRepo.GetAllAsync();
            Tours.Clear();
            foreach (var t in tours) Tours.Add(t);
            ApplyToursFilter();

            var emps = await _empRepo.GetAllAsync();
            Employees.Clear();
            foreach (var e in emps) Employees.Add(e);

            var contracts = await _contractRepo.GetAllAsync();
            Contracts.Clear();
            foreach (var c in contracts) Contracts.Add(c);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки базы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ApplyToursFilter()
    {
        FilteredTours.Clear();
        var q = Tours.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(FilterDestination))
            q = q.Where(t => t.Destination.Contains(FilterDestination, StringComparison.OrdinalIgnoreCase));

        if (FilterMinDays > 0)
            q = q.Where(t => t.DurationDays >= FilterMinDays);

        if (FilterHotelClass > 0)
            q = q.Where(t => t.HotelClass == FilterHotelClass);

        foreach (var t in q) FilteredTours.Add(t);
    }

    // --- Обработка CRUD с авто-обновлением ---

    private async Task AddTour()
    {
        var tour = new Tour { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(7), HotelClass = 4 };
        var dlg = new TourEditDialog(tour);
        if (dlg.ShowDialog() == true)
        {
            try
            {
                tour.Id = await _tourRepo.AddAsync(tour);
                Tours.Insert(0, tour);
                ApplyToursFilter();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка ввода"); }
        }
    }

    private async Task EditTour()
    {
        if (SelectedTour == null) return;
        var clone = new Tour
        {
            Id = SelectedTour.Id,
            TourNumber = SelectedTour.TourNumber,
            TourName = SelectedTour.TourName,
            DurationDays = SelectedTour.DurationDays,
            Destination = SelectedTour.Destination,
            StartDate = SelectedTour.StartDate,
            EndDate = SelectedTour.EndDate,
            HotelClass = SelectedTour.HotelClass,
            AvailableSeats = SelectedTour.AvailableSeats
        };

        var dlg = new TourEditDialog(clone);
        if (dlg.ShowDialog() == true)
        {
            try
            {
                await _tourRepo.UpdateAsync(clone);
                var idx = Tours.IndexOf(SelectedTour);
                Tours[idx] = clone;
                ApplyToursFilter();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
    }

    private async Task DeleteTour()
    {
        if (SelectedTour == null) return;
        if (MessageBox.Show("Удалить выбранный тур?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
                await _tourRepo.DeleteAsync(SelectedTour.Id);
                Tours.Remove(SelectedTour);
                ApplyToursFilter();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка удаления"); }
        }
    }

    private async Task AddEmployee()
    {
        var emp = new Employee();
        var dlg = new EmployeeEditDialog(emp);
        if (dlg.ShowDialog() == true)
        {
            try
            {
                emp.Id = await _empRepo.AddAsync(emp);
                Employees.Add(emp);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
    }

    private async Task EditEmployee()
    {
        if (SelectedEmployee == null) return;
        var clone = new Employee
        {
            Id = SelectedEmployee.Id,
            FullName = SelectedEmployee.FullName,
            Address = SelectedEmployee.Address,
            ExperienceYears = SelectedEmployee.ExperienceYears
        };
        var dlg = new EmployeeEditDialog(clone);
        if (dlg.ShowDialog() == true)
        {
            try
            {
                await _empRepo.UpdateAsync(clone);
                var idx = Employees.IndexOf(SelectedEmployee);
                Employees[idx] = clone;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
    }

    private async Task DeleteEmployee()
    {
        if (SelectedEmployee == null) return;
        if (MessageBox.Show("Удалить сотрудника?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
                await _empRepo.DeleteAsync(SelectedEmployee.Id);
                Employees.Remove(SelectedEmployee);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
    }

    private async Task AddContract()
    {
        var contract = new Contract { ContractDate = DateTime.Today };
        var dlg = new ContractEditDialog(contract, Tours, Employees);
        if (dlg.ShowDialog() == true)
        {
            try
            {
                contract.Id = await _contractRepo.AddAsync(contract);
                Contracts.Insert(0, contract);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
    }

    private async Task EditContract()
    {
        if (SelectedContract == null) return;
        var clone = new Contract
        {
            Id = SelectedContract.Id,
            ContractNumber = SelectedContract.ContractNumber,
            TourId = SelectedContract.TourId,
            TourName = SelectedContract.TourName,
            EmployeeId = SelectedContract.EmployeeId,
            EmployeeName = SelectedContract.EmployeeName,
            ContractDate = SelectedContract.ContractDate,
            CustomerName = SelectedContract.CustomerName,
            CustomerPhone = SelectedContract.CustomerPhone,
            TotalCost = SelectedContract.TotalCost
        };
        var dlg = new ContractEditDialog(clone, Tours, Employees);
        if (dlg.ShowDialog() == true)
        {
            try
            {
                await _contractRepo.UpdateAsync(clone);
                var idx = Contracts.IndexOf(SelectedContract);
                Contracts[idx] = clone;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
    }

    private async Task DeleteContract()
    {
        if (SelectedContract == null) return;
        if (MessageBox.Show("Удалить договор?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
                await _contractRepo.DeleteAsync(SelectedContract.Id);
                Contracts.Remove(SelectedContract);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
    }

    // --- Обработка Запросов ---

    private async Task LoadQuery1()
    {
        var res = await _queryRepo.GetContractsByDateAsync(SelectedQueryDate);
        QueryDateContracts.Clear();
        foreach (var x in res) QueryDateContracts.Add(x);
    }

    private async Task LoadQuery2()
    {
        var res = await _queryRepo.GetEmployeesWithManyContractsAsync(SelectedMinContracts);
        QueryTopEmployees.Clear();
        foreach (var x in res) QueryTopEmployees.Add(x);
    }

    private async Task LoadQuery3()
    {
        var res = await _queryRepo.GetToursByMinDurationAsync(SelectedMinDuration);
        QueryMinDurationTours.Clear();
        foreach (var x in res) QueryMinDurationTours.Add(x);
    }

    private async Task LoadQuery4()
    {
        var res = await _queryRepo.GetToursByCustomerAsync(SelectedCustomerName);
        QueryCustomerTours.Clear();
        foreach (var x in res) QueryCustomerTours.Add(x);
    }

    private async Task LoadQuery5()
    {
        var res = await _queryRepo.GetEmployeeSalesByDateAsync();
        QueryGroupedSales.Clear();
        foreach (var x in res) QueryGroupedSales.Add(x);
    }

    // --- Экспорт в Word ---

    private void GenerateToursReport()
    {
        var sfd = new SaveFileDialog { Filter = "Word Document (*.docx)|*.docx", FileName = "Отчет_Все_Туры.docx" };
        if (sfd.ShowDialog() == true)
        {
            try
            {
                _reportService.GenerateToursReport(sfd.FileName, Tours);
                MessageBox.Show("Отчет успешно сформирован!", "Успех");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка отчета"); }
        }
    }

    private void GenerateCustomerReport()
    {
        if (string.IsNullOrWhiteSpace(SelectedCustomerName))
        {
            MessageBox.Show("Введите имя заказчика для отчета!", "Внимание");
            return;
        }

        var sfd = new SaveFileDialog { Filter = "Word Document (*.docx)|*.docx", FileName = $"Отчет_Клиент_{SelectedCustomerName}.docx" };
        if (sfd.ShowDialog() == true)
        {
            try
            {
                _reportService.GenerateCustomerToursReport(sfd.FileName, SelectedCustomerName, QueryCustomerTours);
                MessageBox.Show("Отчет успешно сформирован!", "Успех");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка отчета"); }
        }
    }

    private void GenerateGroupedReport()
    {
        var sfd = new SaveFileDialog { Filter = "Word Document (*.docx)|*.docx", FileName = "Группирующий_Отчет_Выручка.docx" };
        if (sfd.ShowDialog() == true)
        {
            try
            {
                _reportService.GenerateEmployeeFinancialSummaryReport(sfd.FileName, QueryGroupedSales);
                MessageBox.Show("Отчет с итогами успешно сформирован!", "Успех");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка отчета"); }
        }
    }
}