using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Contracts
{
    // Interface of service managing countries
    public interface ICountryService
    {
        // Get all countries
        Task<OperationDetails<IEnumerable<Country>>> GetAllAsync();
        // Get one country by id
        Task<OperationDetails<Country>> GetByIdAsync(Guid id);
        // Create country
        Task<OperationDetails<Country>> CreateAsync(Country country);
        // Edit country
        Task<OperationDetails<Country>> EditAsync(Country country);
    }
}
