using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.DataTables;
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
        // Create or Edit country
        Task<OperationDetails<CountryDto>> SaveAsync(CountryDto country);
        // Get countries by conditions
        Task<OperationDetails<DataTablesPagedResults<CountryDto>>> GetAsync(DataTablesParameters table);
    }
}
