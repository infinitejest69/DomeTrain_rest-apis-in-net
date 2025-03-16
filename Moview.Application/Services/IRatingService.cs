using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    public interface IRatingService
    {
        Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cancellationToken = default);

        Task<bool> DeleteRatingAsync(Guid moveId, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid? userId = default, CancellationToken cancellationToken = default);

    }
}
