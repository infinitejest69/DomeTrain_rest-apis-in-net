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
                Genre = request.Genre.ToList(),
                Duration = request.Duration,
                ReleaseYear = request.ReleaseYear,
                Director = request.Director,
                Cast = request.Cast.ToList(),
                Trailer = request.Trailer,
                Image = request.Image
            };
        }
        public static Movie ToMovie(this UpdateMovieRequest request, Guid id)
        {
            return new Movie
            {
                Id= id,
                Title = request.Title,
                Description = request.Description,
                Genre = request.Genre.ToList(),
                Duration = request.Duration,
                ReleaseYear = request.ReleaseYear,
                Director = request.Director,
                Cast = request.Cast.ToList(),
                Trailer = request.Trailer,
                Image = request.Image
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
                Genre = movie.Genre,
                Duration = movie.Duration,
                ReleaseYear = movie.ReleaseYear,
                Director = movie.Director,
                Cast = movie.Cast,
                Trailer = movie.Trailer,
                Image = movie.Image
            };
        }

        public static MoviesResponse ToMoviesResponse(this IEnumerable<Movie> movies)
        {
            return new MoviesResponse
            {
                Items = movies.Select(m => m.ToMovieResponse())
            };
        }
    }
}
