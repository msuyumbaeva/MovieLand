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

        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext context) {
            _context = context;
        }

        public IGenreRepository Genres {
            get { return _genreRepository ?? (_genreRepository = new GenreRepository(_context)); }
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
