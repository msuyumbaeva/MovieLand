using MovieLand.Data.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.Data.Contracts.Repositories
{
    public interface IUnitOfWork
    {
        IGenreRepository Genres { get; }
        ICountryRepository Countries { get; }
        IArtistRepository Artists { get; }
        IMovieRepository Movies { get; }
        ICommentRepository Comments { get; }

        Task<int> CompleteAsync();
        void BeginTransaction();
        void RollbackTransaction();
        void CommitTransaction();
    }
}
