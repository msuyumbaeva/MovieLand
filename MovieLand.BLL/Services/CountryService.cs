using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Builders;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Services
{
    // CountryService class to manage countries
    public class CountryService: BaseService, ICountryService
    {
        public CountryService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork) {
        }

        // Create or Edit country
        public async Task<OperationDetails<CountryDto>> SaveAsync(CountryDto countryDto) {
            try {
                // Check if country already exists
                // Build query
                var queryBuilder = new EntityQueryBuilder<Country>();
                queryBuilder.SetFilter(g => g.Name == countryDto.Name && g.Id != countryDto.Id);
                var countriesWithSameName = await _unitOfWork.Countries.GetAsync(queryBuilder);
                if (countriesWithSameName.Count() > 0) {
                    throw new Exception($"Country with the name {countryDto.Name} is already exists");
                }

                // Map dto to country 
                var country = _mapper.Map<Country>(countryDto);

                // Check if country Id is empty
                if (country.Id == Guid.Empty) {
                    // Add new country
                    country = await _unitOfWork.Countries.AddAsync(country);
                }
                else {
                    // Update existing country
                    _unitOfWork.Countries.Update(country);
                }

                // Save changes
                await _unitOfWork.CompleteAsync();

                // Map country to dto
                countryDto = _mapper.Map<CountryDto>(country);
                return OperationDetails<CountryDto>.Success(countryDto);
            }
            catch (Exception ex) {
                return OperationDetails<CountryDto>.Failure().AddError(ex.Message);
            }
        }

        // Get all countries
        public async Task<OperationDetails<IEnumerable<CountryDto>>> GetAllAsync() {

            try {
                var countries = await _unitOfWork.Countries.GetAllAsync();
                var dto = _mapper.Map<List<CountryDto>>(countries);
                return OperationDetails<IEnumerable<CountryDto>>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<IEnumerable<CountryDto>>.Failure().AddError(ex.Message);
            }
        }

        // Get one country by id
        public async Task<OperationDetails<CountryDto>> GetByIdAsync(Guid id) {
            try {
                var country = await _unitOfWork.Countries.GetByIdAsync(id);
                var dto = _mapper.Map<CountryDto>(country);
                return OperationDetails<CountryDto>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<CountryDto>.Failure().AddError(ex.Message);
            }
        }

        // Get countries by conditions
        public async Task<OperationDetails<DataTablesPagedResults<CountryDto>>> GetAsync(DataTablesParameters table) {
            try {
                CountryDto[] items = null;
                // Get total size
                var size = await _unitOfWork.Countries.CountAsync();

                /// Query building
                var queryBuilder = new EntityQueryBuilder<Country>();

                // Filter
                if (!string.IsNullOrEmpty(table.Search.Value))
                    queryBuilder.SetFilter(m => m.Name.Contains(table.Search.Value));

                // Order
                var order = table.Order[0];
                Expression<Func<Country, string>> orderProperty = null;

                // Order property
                if (table.SortOrder == "Name")
                    orderProperty = m => m.Name;

                // Order direction
                if (order.Dir == DTOrderDir.ASC)
                    queryBuilder.SetOrderBy(m => m.OrderBy(orderProperty));
                else
                    queryBuilder.SetOrderBy(m => m.OrderByDescending(orderProperty));

                // Limit
                queryBuilder.SetLimit(table.Length);

                if (table.Length > 0) {
                    // Offset
                    queryBuilder.SetOffset((table.Start / table.Length) * table.Length);
                }
                /// End Query building

                // Get countries
                var countries = await _unitOfWork.Countries.GetAsync(queryBuilder);
                // Map to dto
                items = _mapper.Map<CountryDto[]>(countries);

                // Return result
                var result = new DataTablesPagedResults<CountryDto> {
                    Items = items,
                    TotalSize = size
                };
                return OperationDetails<DataTablesPagedResults<CountryDto>>.Success(result);
            }
            catch (Exception ex) {
                return OperationDetails<DataTablesPagedResults<CountryDto>>.Failure().AddError(ex.Message);
            }
        }
    }
}
