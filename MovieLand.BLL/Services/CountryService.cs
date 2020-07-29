using Microsoft.EntityFrameworkCore;
using MovieLand.BLL.Contracts;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Services
{
    // CountryService class to manage countries
    public class CountryService: ICountryService
    {
        private readonly AppDbContext _context;

        public CountryService(AppDbContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Create country
        public async Task<OperationDetails<Country>> CreateAsync(Country country) {
            try {
                var countriesWithSameName = await _context.Countries.Where(g => g.Name == country.Name).CountAsync();
                if (countriesWithSameName > 0) {
                    throw new Exception($"Жанр с наименованием {country.Name} уже существует");
                }

                var countryEntry = await _context.Countries.AddAsync(country);
                await _context.SaveChangesAsync();

                return OperationDetails<Country>.Success(countryEntry.Entity);
            }
            catch (Exception ex) {
                return OperationDetails<Country>.Failure().AddError(ex.Message);
            }
        }

        // Edit country
        public async Task<OperationDetails<Country>> EditAsync(Country country) {
            try {
                var dbCountry = await _context.Countries.FindAsync(country.Id);
                if (dbCountry == null) {
                    throw new Exception($"Жанр с Id {country.Id} не найден");
                }

                var countriesWithSameName = await _context.Countries.Where(g => g.Name == country.Name && g.Id != country.Id).CountAsync();
                if (countriesWithSameName > 0) {
                    throw new Exception($"Жанр с наименованием {country.Name} уже существует");
                }

                dbCountry.Name = country.Name;
                var countryEntry = _context.Countries.Update(dbCountry);
                await _context.SaveChangesAsync();

                return OperationDetails<Country>.Success(countryEntry.Entity);
            }
            catch (Exception ex) {
                return OperationDetails<Country>.Failure().AddError(ex.Message);
            }
        }

        // Get all countries
        public async Task<OperationDetails<IEnumerable<Country>>> GetAllAsync() {

            try {
                var countries = await _context.Countries.ToListAsync();
                return OperationDetails<IEnumerable<Country>>.Success(countries);
            }
            catch (Exception ex) {
                return OperationDetails<IEnumerable<Country>>.Failure().AddError(ex.Message);
            }
        }

        // Get one country by id
        public async Task<OperationDetails<Country>> GetByIdAsync(Guid id) {
            try {
                var country = await _context.Countries.FindAsync(id);
                return OperationDetails<Country>.Success(country);
            }
            catch (Exception ex) {
                return OperationDetails<Country>.Failure().AddError(ex.Message);
            }
        }
    }
}
