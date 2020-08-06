using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.Data.Contracts.Repositories
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<Country> GetByIdAsync(Guid id);
    }
}
