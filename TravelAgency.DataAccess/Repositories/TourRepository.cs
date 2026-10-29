using System.Data;
using Microsoft.Data.SqlClient;
using TravelAgency.Domain.Entities;
using TravelAgency.Domain.Interfaces;

namespace TravelAgency.DataAccess.Repositories;

public class TourRepository : ITourRepository
{
    private readonly DbConnectionFactory _factory;

    public TourRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Tour>> GetAllAsync()
    {
        var list = new List<Tour>();
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_GetAllTours", conn) { CommandType = CommandType.StoredProcedure };

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

    public async Task<int> AddAsync(Tour tour)
    {
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_InsertTour", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@TourNumber", tour.TourNumber);
        cmd.Parameters.AddWithValue("@TourName", tour.TourName);
        cmd.Parameters.AddWithValue("@DurationDays", tour.DurationDays);
        cmd.Parameters.AddWithValue("@Destination", tour.Destination);
        cmd.Parameters.AddWithValue("@StartDate", tour.StartDate);
        cmd.Parameters.AddWithValue("@EndDate", tour.EndDate);
        cmd.Parameters.AddWithValue("@HotelClass", tour.HotelClass);
        cmd.Parameters.AddWithValue("@AvailableSeats", tour.AvailableSeats);

        var outputId = new SqlParameter("@NewId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(outputId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return (int)outputId.Value;
    }

    public async Task UpdateAsync(Tour tour)
    {
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_UpdateTour", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@Id", tour.Id);
        cmd.Parameters.AddWithValue("@TourNumber", tour.TourNumber);
        cmd.Parameters.AddWithValue("@TourName", tour.TourName);
        cmd.Parameters.AddWithValue("@DurationDays", tour.DurationDays);
        cmd.Parameters.AddWithValue("@Destination", tour.Destination);
        cmd.Parameters.AddWithValue("@StartDate", tour.StartDate);
        cmd.Parameters.AddWithValue("@EndDate", tour.EndDate);
        cmd.Parameters.AddWithValue("@HotelClass", tour.HotelClass);
        cmd.Parameters.AddWithValue("@AvailableSeats", tour.AvailableSeats);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = (SqlConnection)_factory.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteTour", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}