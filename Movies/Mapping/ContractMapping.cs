using Movies.Application.Models;
using Movies.Contracts.Requests;

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
                ReleaseDate = request.ReleaseDate,
                Director = request.Director,
                Cast = request.Cast.ToList(),
                Trailer = request.Trailer,
                Image = request.Image
            };
        }
    }
}
