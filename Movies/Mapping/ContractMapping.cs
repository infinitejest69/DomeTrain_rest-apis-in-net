using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping
{
    public static class ContractMapping
    {

        public static Movie ToMovie(this CreateMovieRequest request)
        {
            return new Movie
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Duration = request.Duration,
                ReleaseYear = request.ReleaseYear,
                Director = request.Director,
                Trailer = request.Trailer,
                Image = request.Image,
                Genres = request.Genres.ToList()
            };
        }
        public static Movie ToMovie(this UpdateMovieRequest request, Guid id)
        {
            return new Movie
            {
                Id= id,
                Title = request.Title,
                Description = request.Description,
                Duration = request.Duration,
                ReleaseYear = request.ReleaseYear,
                Director = request.Director,
                Trailer = request.Trailer,
                Image = request.Image,
                Genres = request.Genres.ToList()
            };
        }

        public static MovieResponse ToMovieResponse(this Movie movie)
        {
            return new MovieResponse
            {
                Id = movie.Id,
                Slug = movie.Slug,
                Title = movie.Title,
                Description = movie.Description,
                Duration = movie.Duration,
                ReleaseYear = movie.ReleaseYear,
                Director = movie.Director,
                Trailer = movie.Trailer,
                Image = movie.Image,
                Rating = movie.Rating,
                UserRating = movie.UserRating,
                Genres = movie.Genres
            };
        }

        public static MoviesResponse ToMoviesResponse(this IEnumerable<Movie> movies)
        {
            return new MoviesResponse
            {
                Items = movies.Select(m => m.ToMovieResponse())
            };
        }

        public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings)
        {
            return ratings.Select(r => new MovieRatingResponse
            {
                MovieId = r.Movie_Id,
                Rating = r.Rating,
                Slug = r.Slug
            });

        }
    }
}
