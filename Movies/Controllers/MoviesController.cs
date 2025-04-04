﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
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

        [Authorize(AuthConstants.TrustedClaimName)]
        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create(
            [FromBody] CreateMovieRequest request,
            CancellationToken cancellationToken
        )
        {
            var movie = request.ToMovie();

            await _movieService.CreateAsync(movie, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { idOrSlug = movie.Id }, movie);
        }
        
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var user = HttpContext.GetUserId();
            
            var movies = await _movieService.GetAllAsync(user, cancellationToken);
            return Ok(movies.ToMoviesResponse());
        }
        
        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> GetById(
            [FromRoute] string idOrSlug,
            CancellationToken cancellationToken
        )
        {
            var user = HttpContext.GetUserId();
            
            var movie = Guid.TryParse(idOrSlug, out var guid)
                ? await _movieService.GetByIdAsync(guid, user, cancellationToken)
                : await _movieService.GetBySlugAsync(idOrSlug,user, cancellationToken);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie.ToMovieResponse());
        }

        [Authorize(AuthConstants.TrustedClaimName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> UpdateById(
            [FromRoute] Guid id,
            [FromBody] UpdateMovieRequest request,
            CancellationToken cancellationToken
        )
        {
            var movie = request.ToMovie(id);
            var user = HttpContext.GetUserId();
            var updatedMovie = await _movieService.UpdateAsync(movie,user, cancellationToken);
            if (updatedMovie is null)
            {
                return NotFound();
            }

            var response = movie.ToMovieResponse();
            return Ok(response);
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> DeleteById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var deleted = await _movieService.DeleteByIdAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}