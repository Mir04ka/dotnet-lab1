using System.Data;
using Microsoft.Data.SqlClient;
using TravelAgency.Domain.Entities;
using TravelAgency.Domain.Interfaces;

namespace TravelAgency.DataAccess.Repositories;

public class ContractRepository : IContractRepository
{
    private readonly DbConnectionFactory _factory;
    public ContractRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Contract>> GetAllAsync()
    {
        var list = new List<Contract>();
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_GetAllContracts", conn) { CommandType = CommandType.StoredProcedure };

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Contract
            {
                Id = reader.GetInt32("Id"),
                ContractNumber = reader.GetString("ContractNumber"),
                TourId = reader.GetInt32("TourId"),
                TourName = reader.GetString("TourName"),
                ContractDate = reader.GetDateTime("ContractDate"),
                EmployeeId = reader.GetInt32("EmployeeId"),
                EmployeeName = reader.GetString("EmployeeName"),
                CustomerName = reader.GetString("CustomerName"),
                CustomerPhone = reader.GetString("CustomerPhone"),
                TotalCost = reader.GetDecimal("TotalCost")
            });
        }
        return list;
    }

    public async Task<int> AddAsync(Contract c)
    {
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_InsertContract", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@ContractNumber", c.ContractNumber);
        cmd.Parameters.AddWithValue("@TourId", c.TourId);
        cmd.Parameters.AddWithValue("@ContractDate", c.ContractDate);
        cmd.Parameters.AddWithValue("@EmployeeId", c.EmployeeId);
        cmd.Parameters.AddWithValue("@CustomerName", c.CustomerName);
        cmd.Parameters.AddWithValue("@CustomerPhone", c.CustomerPhone);
        cmd.Parameters.AddWithValue("@TotalCost", c.TotalCost);

        var outId = new SqlParameter("@NewId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(outId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return (int)outId.Value;
    }

    public async Task UpdateAsync(Contract c)
    {
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_UpdateContract", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@Id", c.Id);
        cmd.Parameters.AddWithValue("@ContractNumber", c.ContractNumber);
        cmd.Parameters.AddWithValue("@TourId", c.TourId);
        cmd.Parameters.AddWithValue("@ContractDate", c.ContractDate);
        cmd.Parameters.AddWithValue("@EmployeeId", c.EmployeeId);
        cmd.Parameters.AddWithValue("@CustomerName", c.CustomerName);
        cmd.Parameters.AddWithValue("@CustomerPhone", c.CustomerPhone);
        cmd.Parameters.AddWithValue("@TotalCost", c.TotalCost);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteContract", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}