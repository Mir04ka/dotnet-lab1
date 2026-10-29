using System.Windows;
using TravelAgency.Domain.Entities;

namespace TravelAgency.WPF.Views;

public partial class ContractEditDialog : Window
{
    private readonly Contract _contract;

    public ContractEditDialog(Contract contract, IEnumerable<Tour> tours, IEnumerable<Employee> employees)
    {
        InitializeComponent();
        _contract = contract;

        CmbTours.ItemsSource = tours;
        CmbEmployees.ItemsSource = employees;

        TxtNumber.Text = contract.ContractNumber;
        DpDate.SelectedDate = contract.ContractDate;
        TxtCustomer.Text = contract.CustomerName;
        TxtPhone.Text = contract.CustomerPhone;
        TxtCost.Text = contract.TotalCost > 0 ? contract.TotalCost.ToString("F2") : "";

        if (contract.TourId > 0) CmbTours.SelectedValue = contract.TourId;
        if (contract.EmployeeId > 0) CmbEmployees.SelectedValue = contract.EmployeeId;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        // Валидация ввода
        if (string.IsNullOrWhiteSpace(TxtNumber.Text) ||
            CmbTours.SelectedItem == null ||
            CmbEmployees.SelectedItem == null ||
            string.IsNullOrWhiteSpace(TxtCustomer.Text) ||
            !decimal.TryParse(TxtCost.Text, out var cost) || cost < 0)
        {
            MessageBox.Show("Пожалуйста, заполните корректно все поля!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _contract.ContractNumber = TxtNumber.Text.Trim();
        _contract.TourId = (int)CmbTours.SelectedValue;
        _contract.TourName = ((Tour)CmbTours.SelectedItem).TourName;
        _contract.EmployeeId = (int)CmbEmployees.SelectedValue;
        _contract.EmployeeName = ((Employee)CmbEmployees.SelectedItem).FullName;
        _contract.ContractDate = DpDate.SelectedDate ?? DateTime.Today;
        _contract.CustomerName = TxtCustomer.Text.Trim();
        _contract.CustomerPhone = TxtPhone.Text.Trim();
        _contract.TotalCost = cost;

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}