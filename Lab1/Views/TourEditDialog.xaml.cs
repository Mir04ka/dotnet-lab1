using System.Windows;
using TravelAgency.Domain.Entities;

namespace TravelAgency.WPF.Views;

public partial class TourEditDialog : Window
{
    private readonly Tour _tour;

    public TourEditDialog(Tour tour)
    {
        InitializeComponent();
        _tour = tour;

        TxtTourNum.Text = tour.TourNumber;
        TxtTourName.Text = tour.TourName;
        TxtDestination.Text = tour.Destination;
        TxtDays.Text = tour.DurationDays > 0 ? tour.DurationDays.ToString() : "7";
        DpStart.SelectedDate = tour.StartDate;
        DpEnd.SelectedDate = tour.EndDate;
        TxtHotelClass.Text = tour.HotelClass.ToString();
        TxtSeats.Text = tour.AvailableSeats.ToString();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtTourNum.Text) ||
            string.IsNullOrWhiteSpace(TxtTourName.Text) ||
            !int.TryParse(TxtDays.Text, out var days) || days <= 0 ||
            !int.TryParse(TxtHotelClass.Text, out var hotelClass) || hotelClass < 1 || hotelClass > 5 ||
            !int.TryParse(TxtSeats.Text, out var seats) || seats < 0)
        {
            MessageBox.Show("Проверьте правильность введенных числовых значений (Дни > 0, Отель 1-5, Места >= 0)!", "Ошибка валидации");
            return;
        }

        _tour.TourNumber = TxtTourNum.Text.Trim();
        _tour.TourName = TxtTourName.Text.Trim();
        _tour.Destination = TxtDestination.Text.Trim();
        _tour.DurationDays = days;
        _tour.StartDate = DpStart.SelectedDate ?? DateTime.Today;
        _tour.EndDate = DpEnd.SelectedDate ?? DateTime.Today.AddDays(days);
        _tour.HotelClass = hotelClass;
        _tour.AvailableSeats = seats;

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}