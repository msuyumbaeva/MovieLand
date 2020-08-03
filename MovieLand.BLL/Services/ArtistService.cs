using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Artist;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Services
{
    public class ArtistService : IArtistService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ArtistService(AppDbContext context, IMapper mapper) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Create artist
        public async Task<OperationDetails<ArtistDto>> CreateAsync(ArtistDto artistDto) {
            try {
                var artistsWithSameName = await _context.Artists.Where(g => g.Name == artistDto.Name).CountAsync();
                if (artistsWithSameName > 0) {
                    throw new Exception($"Artist with name {artistDto.Name} is already exists");
                }

                var artist = _mapper.Map<Artist>(artistDto);
                var artistEntry = await _context.Artists.AddAsync(artist);
                await _context.SaveChangesAsync();

                artistDto = _mapper.Map<ArtistDto>(artistEntry.Entity);
                return OperationDetails<ArtistDto>.Success(artistDto);
            }
            catch (Exception ex) {
                return OperationDetails<ArtistDto>.Failure().AddError(ex.Message);
            }
        }

        // Edit artist
        public async Task<OperationDetails<ArtistDto>> EditAsync(ArtistDto artistDto) {
            try {
                var dbArtist = await _context.Artists.FindAsync(artistDto.Id);
                if (dbArtist == null) {
                    throw new Exception($"Artist with Id {artistDto.Id} was not found");
                }

                var artistsWithSameName = await _context.Artists.Where(g => g.Name == artistDto.Name && g.Id != artistDto.Id).CountAsync();
                if (artistsWithSameName > 0) {
                    throw new Exception($"Artist with name {artistDto.Name} is already exists");
                }

                dbArtist.Name = artistDto.Name;
                var artistEntry = _context.Artists.Update(dbArtist);
                await _context.SaveChangesAsync();

                artistDto = _mapper.Map<ArtistDto>(artistEntry.Entity);
                return OperationDetails<ArtistDto>.Success(artistDto);
            }
            catch (Exception ex) {
                return OperationDetails<ArtistDto>.Failure().AddError(ex.Message);
            }
        }

        // Get all artists
        public async Task<OperationDetails<IEnumerable<ArtistDto>>> GetAllAsync() {

            try {
                var artists = await _context.Artists.ProjectTo<ArtistDto>(_mapper.ConfigurationProvider).ToListAsync();
                return OperationDetails<IEnumerable<ArtistDto>>.Success(artists);
            }
            catch (Exception ex) {
                return OperationDetails<IEnumerable<ArtistDto>>.Failure().AddError(ex.Message);
            }
        }

        // Get one artist by id
        public async Task<OperationDetails<ArtistDto>> GetByIdAsync(Guid id) {
            try {
                var artist = await _context.Artists.FindAsync(id);
                var dto = _mapper.Map<ArtistDto>(artist);
                return OperationDetails<ArtistDto>.Success(dto);
            }
            catch (Exception ex) {
                return OperationDetails<ArtistDto>.Failure().AddError(ex.Message);
            }
        }
    }
}
