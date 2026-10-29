namespace TravelAgency.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}

public class Tour
{
    public int Id { get; set; }
    public string TourNumber { get; set; } = string.Empty;
    public string TourName { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string Destination { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(7);
    public int HotelClass { get; set; } = 4;
    public int AvailableSeats { get; set; }
}

public class Contract
{
    public int Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public int TourId { get; set; }
    public string TourName { get; set; } = string.Empty; // Nav
    public DateTime ContractDate { get; set; } = DateTime.Today;
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty; // Nav
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
}