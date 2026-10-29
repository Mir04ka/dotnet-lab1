using System.Data;
using Microsoft.Data.SqlClient;
using TravelAgency.Domain.Entities;
using TravelAgency.Domain.Interfaces;

namespace TravelAgency.DataAccess.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly DbConnectionFactory _factory;
    public EmployeeRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        var list = new List<Employee>();
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_GetAllEmployees", conn) { CommandType = CommandType.StoredProcedure };

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Employee
            {
                Id = reader.GetInt32("Id"),
                FullName = reader.GetString("FullName"),
                Address = reader.GetString("Address"),
                ExperienceYears = reader.GetInt32("ExperienceYears")
            });
        }
        return list;
    }

    public async Task<int> AddAsync(Employee emp)
    {
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_InsertEmployee", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@FullName", emp.FullName);
        cmd.Parameters.AddWithValue("@Address", emp.Address);
        cmd.Parameters.AddWithValue("@ExperienceYears", emp.ExperienceYears);

        var outputId = new SqlParameter("@NewId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(outputId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return (int)outputId.Value;
    }

    public async Task UpdateAsync(Employee emp)
    {
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_UpdateEmployee", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@Id", emp.Id);
        cmd.Parameters.AddWithValue("@FullName", emp.FullName);
        cmd.Parameters.AddWithValue("@Address", emp.Address);
        cmd.Parameters.AddWithValue("@ExperienceYears", emp.ExperienceYears);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteEmployee", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}