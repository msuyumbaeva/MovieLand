using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Dtos.Genre;
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
    // GenreService class to manage genres
    public class GenreService : BaseService, IGenreService
    {

        public GenreService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper,unitOfWork) {
        }

        // Create or edit genre
        public async Task<OperationDetails<GenreDto>> SaveAsync(GenreDto genreDto) {
            try {
                // Change name to lower case
                genreDto.Name = genreDto.Name.ToLower();

                // Check if genre already exists
                // Build query
                var queryBuilder = new EntityQueryBuilder<Genre>();
                queryBuilder.SetFilter(g => g.Name == genreDto.Name && g.Id != genreDto.Id);
                var genresWithSameName = await _unitOfWork.Genres.GetAsync(queryBuilder);
                if (genresWithSameName.Count() > 0) {
                    throw new Exception($"Genre with the name {genreDto.Name} is already exists");
                }

                // Map dto to genre 
                var genre = _mapper.Map<Genre>(genreDto);

                // Check if genre Id is empty
                if (genre.Id == Guid.Empty) {
                    // Add new genre
                    genre = await _unitOfWork.Genres.AddAsync(genre);
                }
                else {
                    // Update existing genre
                    _unitOfWork.Genres.Update(genre);
                }

                // Save changes
                await _unitOfWork.CompleteAsync();

                // Map genre to dto
                genreDto = _mapper.Map<GenreDto>(genre);
                return OperationDetails<GenreDto>.Success(genreDto);
            }
            catch (Exception ex) {
                return OperationDetails<GenreDto>.Failure().AddError(ex.Message);
            }
        }

        // Get all genres
        public async Task<OperationDetails<IEnumerable<GenreDto>>> GetAllAsync() {
            try {
                var genres = await _unitOfWork.Genres.GetAllAsync();
                var dto = _mapper.Map<List<GenreDto>>(genres);
                return OperationDetails<IEnumerable<GenreDto>>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<IEnumerable<GenreDto>>.Failure().AddError(ex.Message);
            }
        }

        // Get one genre by id
        public async Task<OperationDetails<GenreDto>> GetByIdAsync(Guid id) {
            try {
                var genre = await _unitOfWork.Genres.GetByIdAsync(id);
                var dto = _mapper.Map<GenreDto>(genre);
                return OperationDetails<GenreDto>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<GenreDto>.Failure().AddError(ex.Message);
            }
        }

        // Get genres by conditions
        public async Task<OperationDetails<DataTablesPagedResults<GenreDto>>> GetAsync(DataTablesParameters table) {
            try {
                GenreDto[] items = null;
                // Get total size
                var size = await _unitOfWork.Genres.CountAsync();

                /// Query building
                var queryBuilder = new EntityQueryBuilder<Genre>();

                // Filter
                if (!string.IsNullOrEmpty(table.Search.Value))
                    queryBuilder.SetFilter(m => m.Name.Contains(table.Search.Value));

                // Order
                var order = table.Order[0];
                Expression<Func<Genre,string>> orderProperty = null;

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

                // Offset
                queryBuilder.SetOffset((table.Start / table.Length) * table.Length);
                /// End Query building

                // Get genres
                var genres = await _unitOfWork.Genres.GetAsync(queryBuilder);
                // Map to dto
                items = _mapper.Map<GenreDto[]>(genres);

                // Return result
                var result = new DataTablesPagedResults<GenreDto> {
                    Items = items,
                    TotalSize = size
                };
                return OperationDetails<DataTablesPagedResults<GenreDto>>.Success(result);
            }
            catch (Exception ex) {
                return OperationDetails<DataTablesPagedResults<GenreDto>>.Failure().AddError(ex.Message);
            }
        }
    }
}
