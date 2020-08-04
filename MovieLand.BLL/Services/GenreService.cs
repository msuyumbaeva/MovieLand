using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Dtos.Genre;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Services
{
    // GenreService class to manage genres
    public class GenreService : IGenreService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GenreService(AppDbContext context, IMapper mapper) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Create genre
        public async Task<OperationDetails<GenreDto>> CreateAsync(GenreDto genreDto) {
            try {
                genreDto.Name = genreDto.Name.ToLower();

                var genresWithSameName = await _context.Genres.Where(g => g.Name == genreDto.Name).CountAsync();
                if(genresWithSameName > 0) {
                    throw new Exception($"Genre with name {genreDto.Name} is already exists");
                }

                var genre = _mapper.Map<Genre>(genreDto);
                var genreEntry = await _context.Genres.AddAsync(genre);
                await _context.SaveChangesAsync();

                genreDto = _mapper.Map<GenreDto>(genreEntry.Entity);
                return OperationDetails<GenreDto>.Success(genreDto);
            }
            catch (Exception ex) {
                return OperationDetails<GenreDto>.Failure().AddError(ex.Message);
            }
        }

        // Edit genre
        public async Task<OperationDetails<GenreDto>> EditAsync(GenreDto genreDto) {
            try {
                var dbGenre = await _context.Genres.FindAsync(genreDto.Id);
                if(dbGenre == null) {
                    throw new Exception($"Genre with Id {genreDto.Id} was not found");
                }

                genreDto.Name = genreDto.Name.ToLower();

                var genresWithSameName = await _context.Genres.Where(g => g.Name == genreDto.Name && g.Id != genreDto.Id).CountAsync();
                if (genresWithSameName > 0) {
                    throw new Exception($"Genre with name {genreDto.Name} is already exists");
                }

                dbGenre.Name = genreDto.Name;
                var genreEntry = _context.Genres.Update(dbGenre);
                await _context.SaveChangesAsync();

                genreDto = _mapper.Map<GenreDto>(genreEntry.Entity);
                return OperationDetails<GenreDto>.Success(genreDto);
            }
            catch (Exception ex) {
                return OperationDetails<GenreDto>.Failure().AddError(ex.Message);
            }
        }

        // Get all genres
        public async Task<OperationDetails<IEnumerable<GenreDto>>> GetAllAsync() {
            try {
                var genres = await _context.Genres.ProjectTo<GenreDto>(_mapper.ConfigurationProvider).ToListAsync();
                return OperationDetails<IEnumerable<GenreDto>>.Success(genres);
            }
            catch (Exception ex) {
                return OperationDetails<IEnumerable<GenreDto>>.Failure().AddError(ex.Message);
            }
        }

        // Get one genre by id
        public async Task<OperationDetails<GenreDto>> GetByIdAsync(Guid id) {
            try {
                var genre = await _context.Genres.FindAsync(id);
                var dto = _mapper.Map<GenreDto>(genre);
                return OperationDetails<GenreDto>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<GenreDto>.Failure().AddError(ex.Message);
            }
        }

        public async Task<OperationDetails<DataTablesPagedResults<GenreDto>>> GetDataAsync(DataTablesParameters table) {
            try {
                GenreDto[] items = null;
                IQueryable<Genre> query = _context.Genres;

                // Filter
                if (!string.IsNullOrEmpty(table.Search.Value)) {
                    query = query.Where(g => g.Name.Contains(table.Search.Value));
                }

                // Sorting
                if (table.Order[0].Dir == DTOrderDir.ASC)
                    query = query.OrderBy(m => m.Name);
                else
                    query = query.OrderByDescending(m => m.Name);

                // Get total size
                var size = await query.CountAsync();

                // Pagination
                if (table.Length > 0) {
                    items = await query
                    .Skip((table.Start / table.Length) * table.Length)
                    .Take(table.Length)
                    .ProjectTo<GenreDto>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
                }
                else {
                    items = await query
                    .ProjectTo<GenreDto>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
                }

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
