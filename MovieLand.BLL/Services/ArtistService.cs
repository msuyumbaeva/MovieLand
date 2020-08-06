using AutoMapper;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Artist;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.Data.Builders;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MovieLand.BLL.Services
{
    public class ArtistService : BaseService, IArtistService
    {
        public ArtistService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork) {
        }

        // Create and Edit artist
        public async Task<OperationDetails<ArtistDto>> SaveAsync(ArtistDto artistDto) {
            try {
                // Check if artist already exists
                // Build query
                var queryBuilder = new EntityQueryBuilder<Artist>();
                queryBuilder.SetFilter(g => g.Name == artistDto.Name && g.Id != artistDto.Id);
                var artistsWithSameName = await _unitOfWork.Artists.GetAsync(queryBuilder);
                if (artistsWithSameName.Count() > 0) {
                    throw new Exception($"Artist with name {artistDto.Name} is already exists");
                }

                // Map dto to artist
                var artist = _mapper.Map<Artist>(artistDto);

                // Check if artist Id is empty
                if (artist.Id == Guid.Empty) {
                    // Add new artist
                    artist = await _unitOfWork.Artists.AddAsync(artist);
                }
                else {
                    // Update existing artist
                    _unitOfWork.Artists.Update(artist);
                }

                // Save changes
                await _unitOfWork.CompleteAsync();

                // Map artist to dto
                artistDto = _mapper.Map<ArtistDto>(artist);
                return OperationDetails<ArtistDto>.Success(artistDto);
            }
            catch (Exception ex) {
                return OperationDetails<ArtistDto>.Failure().AddError(ex.Message);
            }
        }

        // Get all artists
        public async Task<OperationDetails<IEnumerable<ArtistDto>>> GetAllAsync() {

            try {
                var artists = await _unitOfWork.Artists.GetAllAsync();
                var dto = _mapper.Map<List<ArtistDto>>(artists);
                return OperationDetails<IEnumerable<ArtistDto>>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<IEnumerable<ArtistDto>>.Failure().AddError(ex.Message);
            }
        }

        // Get one artist by id
        public async Task<OperationDetails<ArtistDto>> GetByIdAsync(Guid id) {
            try {
                var artist = await _unitOfWork.Artists.GetByIdAsync(id);
                var dto = _mapper.Map<ArtistDto>(artist);
                return OperationDetails<ArtistDto>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<ArtistDto>.Failure().AddError(ex.Message);
            }
        }

        // Get artists by conditions
        public async Task<OperationDetails<DataTablesPagedResults<ArtistDto>>> GetAsync(DataTablesParameters table) {
            try {
                ArtistDto[] items = null;
                // Get total size
                var size = await _unitOfWork.Artists.CountAsync();

                /// Query building
                var queryBuilder = new EntityQueryBuilder<Artist>();

                // Filter
                if (!string.IsNullOrEmpty(table.Search.Value))
                    queryBuilder.SetFilter(m => m.Name.Contains(table.Search.Value));

                // Order
                var order = table.Order[0];
                Expression<Func<Artist, string>> orderProperty = null;

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

                // Get artists
                var artists = await _unitOfWork.Artists.GetAsync(queryBuilder);
                // Map to dto
                items = _mapper.Map<ArtistDto[]>(artists);

                // Return result
                var result = new DataTablesPagedResults<ArtistDto> {
                    Items = items,
                    TotalSize = size
                };
                return OperationDetails<DataTablesPagedResults<ArtistDto>>.Success(result);
            }
            catch (Exception ex) {
                return OperationDetails<DataTablesPagedResults<ArtistDto>>.Failure().AddError(ex.Message);
            }
        }
    }
}
