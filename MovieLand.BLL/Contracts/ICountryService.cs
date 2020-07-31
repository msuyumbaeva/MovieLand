using MovieLand.BLL.Dtos.Country;
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
        Task<OperationDetails<IEnumerable<CountryDto>>> GetAllAsync();
        // Get one country by id
        Task<OperationDetails<CountryDto>> GetByIdAsync(Guid id);
        // Create country
        Task<OperationDetails<CountryDto>> CreateAsync(CountryDto country);
        // Edit country
        Task<OperationDetails<CountryDto>> EditAsync(CountryDto country);
    }
}
