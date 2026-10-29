namespace TravelAgency.Domain.DTOs;

public class CustomerTourDto
{
    public string TourName { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime ContractDate { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
}

public class EmployeeTopDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public int ContractsCount { get; set; }
}

public class EmployeeDailySalesDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime ContractDate { get; set; }
    public decimal DailyTotal { get; set; }
}