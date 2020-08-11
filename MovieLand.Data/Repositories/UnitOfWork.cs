using Microsoft.EntityFrameworkCore.Storage;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IGenreRepository _genreRepository;
        private ICountryRepository _countryRepository;
        private IArtistRepository _artistRepository;
        private IMovieRepository _movieRepository;
        private ICommentRepository _commentRepository;
        private IStarRatingRepository _starRatingRepository;

        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext context) {
            _context = context;
        }

        public IGenreRepository Genres {
            get { return _genreRepository ?? (_genreRepository = new GenreRepository(_context)); }
        }

        public ICountryRepository Countries {
            get { return _countryRepository ?? (_countryRepository = new CountryRepository(_context)); }
        }

        public IArtistRepository Artists {
            get { return _artistRepository ?? (_artistRepository = new ArtistRepository(_context)); }
        }

        public IMovieRepository Movies {
            get { return _movieRepository ?? (_movieRepository = new MovieRepository(_context)); }
        }

        public ICommentRepository Comments {
            get { return _commentRepository ?? (_commentRepository = new CommentRepository(_context)); }
        }

        public IStarRatingRepository StarRatings {
            get { return _starRatingRepository ?? (_starRatingRepository = new StarRatingRepository(_context)); }
        }

        public Task<int> CompleteAsync() {
            return _context.SaveChangesAsync();
        }

        public void BeginTransaction() {
            _transaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction() {
            if (_transaction == null) return;

            _transaction.Commit();
            _transaction.Dispose();

            _transaction = null;
        }

        public void RollbackTransaction() {
            if (_transaction == null) return;

            _transaction.Rollback();
            _transaction.Dispose();

            _transaction = null;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
