using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;

        }

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
        {

            var movie = request.ToMovie();

            await _movieService.CreateAsync(movie);
            return CreatedAtAction(nameof(GetAll), new { idOrSlug = movie.Id }, movie);

        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _movieService.GetAllAsync();
            return Ok(movies.ToMoviesResponse());
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> GetById([FromRoute] string idOrSlug)
        {

            var movie = Guid.TryParse(idOrSlug, out var guid) ? await _movieService.GetByIdAsync(guid) : await _movieService.GetBySlugAsync(idOrSlug);

            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie.ToMovieResponse());
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> UpdateById([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
        {
            var movie = request.ToMovie(id);
            var updatedMovie =  await _movieService.UpdateAsync(movie);
            if (updatedMovie is null)
            {
                return NotFound();
            }
            var response = movie.ToMovieResponse();
            return Ok(response);

        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id)
        {
            var deleted = await _movieService.DeleteByIdAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok();

        }


    }
}
