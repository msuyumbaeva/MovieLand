using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Country;
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
        private readonly IMapper _mapper;

        public CountryService(AppDbContext context, IMapper mapper) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Create country
        public async Task<OperationDetails<CountryDto>> CreateAsync(CountryDto countryDto) {
            try {
                var countriesWithSameName = await _context.Countries.Where(g => g.Name == countryDto.Name).CountAsync();
                if (countriesWithSameName > 0) {
                    throw new Exception($"Country with name {countryDto.Name} is already exists");
                }

                var country = _mapper.Map<Country>(countryDto);
                var countryEntry = await _context.Countries.AddAsync(country);
                await _context.SaveChangesAsync();

                countryDto = _mapper.Map<CountryDto>(countryEntry.Entity);
                return OperationDetails<CountryDto>.Success(countryDto);
            }
            catch (Exception ex) {
                return OperationDetails<CountryDto>.Failure().AddError(ex.Message);
            }
        }

        // Edit country
        public async Task<OperationDetails<CountryDto>> EditAsync(CountryDto countryDto) {
            try {
                var dbCountry = await _context.Countries.FindAsync(countryDto.Id);
                if (dbCountry == null) {
                    throw new Exception($"Country with Id {countryDto.Id} was not found");
                }

                var countriesWithSameName = await _context.Countries.Where(g => g.Name == countryDto.Name && g.Id != countryDto.Id).CountAsync();
                if (countriesWithSameName > 0) {
                    throw new Exception($"Country with name {countryDto.Name} is already exists");
                }

                dbCountry.Name = countryDto.Name;
                var countryEntry = _context.Countries.Update(dbCountry);
                await _context.SaveChangesAsync();

                countryDto = _mapper.Map<CountryDto>(countryEntry.Entity);
                return OperationDetails<CountryDto>.Success(countryDto);
            }
            catch (Exception ex) {
                return OperationDetails<CountryDto>.Failure().AddError(ex.Message);
            }
        }

        // Get all countries
        public async Task<OperationDetails<IEnumerable<CountryDto>>> GetAllAsync() {

            try {
                var countries = await _context.Countries.ProjectTo<CountryDto>(_mapper.ConfigurationProvider).ToListAsync();
                return OperationDetails<IEnumerable<CountryDto>>.Success(countries);
            }
            catch (Exception ex) {
                return OperationDetails<IEnumerable<CountryDto>>.Failure().AddError(ex.Message);
            }
        }

        // Get one country by id
        public async Task<OperationDetails<CountryDto>> GetByIdAsync(Guid id) {
            try {
                var country = await _context.Countries.FindAsync(id);
                var dto = _mapper.Map<CountryDto>(country);
                return OperationDetails<CountryDto>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<CountryDto>.Failure().AddError(ex.Message);
            }
        }
    }
}
