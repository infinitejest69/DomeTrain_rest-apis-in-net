using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IValidator<Movie> _movieValidator;

        public MovieService(IMovieRepository movieRepository, IValidator<Movie> validator)
        {
            _movieRepository = movieRepository;
            _movieValidator = validator;
        }

        public async Task<bool> CreateAsync(
            Movie movie,
            CancellationToken cancellationToken = default
        )
        {
            await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: cancellationToken);
            return await _movieRepository.CreateAsync(movie, cancellationToken);
        }

        public Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _movieRepository.DeleteByIdAsync(id, cancellationToken);
        }

        public Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _movieRepository.ExistsByIdAsync(id, cancellationToken);
        }

        public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _movieRepository.GetAllAsync( cancellationToken);
        }

        public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _movieRepository.GetByIdAsync(id, cancellationToken);
        }

        public Task<Movie?> GetBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default
        )
        {
            return _movieRepository.GetBySlugAsync(slug, cancellationToken);
        }

        public async Task<Movie?> UpdateAsync(
            Movie movie,
            CancellationToken cancellationToken = default
        )
        {
            await _movieValidator.ValidateAndThrowAsync(movie,cancellationToken);
            var moveExists = await _movieRepository.ExistsByIdAsync(movie.Id, cancellationToken);
            if (!moveExists)
            {
                return null;
            }
            await _movieRepository.UpdateAsync(movie, cancellationToken);
            return movie;
        }
    }
}
