using System.Windows;
using TravelAgency.Domain.Entities;

namespace TravelAgency.WPF.Views;

public partial class EmployeeEditDialog : Window
{
    private readonly Employee _employee;

    public EmployeeEditDialog(Employee employee)
    {
        InitializeComponent();
        _employee = employee;

        TxtName.Text = employee.FullName;
        TxtAddress.Text = employee.Address;
        TxtExp.Text = employee.ExperienceYears.ToString();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtName.Text) || !int.TryParse(TxtExp.Text, out var exp) || exp < 0)
        {
            MessageBox.Show("Введите корректные ФИО и стаж!", "Ошибка");
            return;
        }

        _employee.FullName = TxtName.Text.Trim();
        _employee.Address = TxtAddress.Text.Trim();
        _employee.ExperienceYears = exp;

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}