using System.Data;
using Microsoft.Data.SqlClient;
using TravelAgency.Domain.DTOs;
using TravelAgency.Domain.Entities;
using TravelAgency.Domain.Interfaces;

namespace TravelAgency.DataAccess.Repositories;

public class QueryRepository : IQueryRepository
{
    private readonly DbConnectionFactory _factory;
    public QueryRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Contract>> GetContractsByDateAsync(DateTime date)
    {
        var list = new List<Contract>();
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_GetContractsByDate", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@ContractDate", date.Date);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Contract
            {
                Id = reader.GetInt32("Id"),
                ContractNumber = reader.GetString("ContractNumber"),
                TourName = reader.GetString("TourName"),
                ContractDate = reader.GetDateTime("ContractDate"),
                EmployeeName = reader.GetString("EmployeeName"),
                CustomerName = reader.GetString("CustomerName"),
                CustomerPhone = reader.GetString("CustomerPhone"),
                TotalCost = reader.GetDecimal("TotalCost")
            });
        }
        return list;
    }

    public async Task<IEnumerable<EmployeeTopDto>> GetEmployeesWithManyContractsAsync(int minContracts)
    {
        var list = new List<EmployeeTopDto>();
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_GetEmployeesWithManyContracts", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@MinContractsCount", minContracts);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new EmployeeTopDto
            {
                Id = reader.GetInt32("Id"),
                FullName = reader.GetString("FullName"),
                Address = reader.GetString("Address"),
                ExperienceYears = reader.GetInt32("ExperienceYears"),
                ContractsCount = reader.GetInt32("ContractsCount")
            });
        }
        return list;
    }

    public async Task<IEnumerable<Tour>> GetToursByMinDurationAsync(int minDays)
    {
        var list = new List<Tour>();
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_GetToursByMinDuration", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@MinDuration", minDays);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Tour
            {
                Id = reader.GetInt32("Id"),
                TourNumber = reader.GetString("TourNumber"),
                TourName = reader.GetString("TourName"),
                DurationDays = reader.GetInt32("DurationDays"),
                Destination = reader.GetString("Destination"),
                StartDate = reader.GetDateTime("StartDate"),
                EndDate = reader.GetDateTime("EndDate"),
                HotelClass = reader.GetInt32("HotelClass"),
                AvailableSeats = reader.GetInt32("AvailableSeats")
            });
        }
        return list;
    }

    public async Task<IEnumerable<CustomerTourDto>> GetToursByCustomerAsync(string customerName)
    {
        var list = new List<CustomerTourDto>();
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_GetToursByCustomer", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@CustomerName", customerName ?? string.Empty);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new CustomerTourDto
            {
                TourName = reader.GetString("TourName"),
                DurationDays = reader.GetInt32("DurationDays"),
                TotalCost = reader.GetDecimal("TotalCost"),
                ContractDate = reader.GetDateTime("ContractDate"),
                ContractNumber = reader.GetString("ContractNumber")
            });
        }
        return list;
    }

    public async Task<IEnumerable<EmployeeDailySalesDto>> GetEmployeeSalesByDateAsync()
    {
        var list = new List<EmployeeDailySalesDto>();
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_GetEmployeeSalesByDate", conn) { CommandType = CommandType.StoredProcedure };

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new EmployeeDailySalesDto
            {
                EmployeeName = reader.GetString("EmployeeName"),
                ContractDate = reader.GetDateTime("ContractDate"),
                DailyTotal = reader.GetDecimal("DailyTotal")
            });
        }
        return list;
    }
}