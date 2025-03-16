using Movies.Application.Models;
using Movies.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IMovieRepository _movieRepository;
        public RatingService(IRatingRepository ratingRepository,IMovieRepository movieRepository )
        {
            _ratingRepository = ratingRepository;
            _movieRepository = movieRepository;

        }

        public async Task<bool> DeleteRatingAsync(Guid moveId, Guid userId, CancellationToken cancellationToken = default)
        {
            var movie = await _movieRepository.ExistsByIdAsync(moveId, userId, cancellationToken);
            if (!movie)
            {
                return false;
            }
            return await _ratingRepository.DeleteRatingAsync(moveId, userId, cancellationToken);

        }

        public Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid? userId = null, CancellationToken cancellationToken = default)
        {
            var ratings = _ratingRepository.GetRatingsForUserAsync(userId, cancellationToken);
            return ratings;
        }

        public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cancellationToken = default)
        {
            if (rating is <= 0 or > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");
            }
            var movie = await _movieRepository.ExistsByIdAsync(movieId,userId, cancellationToken);
            if (!movie)
            {
                return false;
            }
            return await _ratingRepository.RateMovieAsync(movieId, rating, userId, cancellationToken);
        }
    }
}
