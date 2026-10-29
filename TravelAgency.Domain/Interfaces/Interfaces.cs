using TravelAgency.Domain.Entities;
using TravelAgency.Domain.DTOs;

namespace TravelAgency.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<int> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public interface ITourRepository : IRepository<Tour> { }
public interface IEmployeeRepository : IRepository<Employee> { }
public interface IContractRepository : IRepository<Contract> { }

public interface IQueryRepository
{
    Task<IEnumerable<Contract>> GetContractsByDateAsync(DateTime date);
    Task<IEnumerable<EmployeeTopDto>> GetEmployeesWithManyContractsAsync(int minContracts);
    Task<IEnumerable<Tour>> GetToursByMinDurationAsync(int minDays);
    Task<IEnumerable<CustomerTourDto>> GetToursByCustomerAsync(string customerName);
    Task<IEnumerable<EmployeeDailySalesDto>> GetEmployeeSalesByDateAsync();
}