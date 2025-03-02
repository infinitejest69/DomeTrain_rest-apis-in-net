using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MovieRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> CreateAsync(
            Movie movie,
            CancellationToken cancellationToken = default
        )
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();

            var result = await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                    insert into movies (id,slug,title,description,duration,releaseYear,director,trailer,image)
                    values (@Id,@Slug,@Title,@Description,@Duration,@ReleaseYear,@Director,@Trailer,@Image)
                    """,
                    movie,
                    cancellationToken: cancellationToken
                )
            );

            if (result > 0)
            {
                foreach (var genre in movie.Genre)
                {
                    await connection.ExecuteAsync(
                        new CommandDefinition(
                            """
                            insert into genres (movie_id, name)
                            values (@MovieId, @Name)
                            """,
                            new { MovieId = movie.Id, Name = genre },
                            cancellationToken: cancellationToken
                        )
                    );
                }
                foreach (var cast in movie.Cast)
                {
                    await connection.ExecuteAsync(
                        new CommandDefinition(
                            """
                            insert into "cast" (movie_id, name)
                            values (@MovieId, @Name)
                            """,
                            new { MovieId = movie.Id, Name = cast },
                            cancellationToken: cancellationToken
                        )
                    );
                }
            }
            transaction.Commit();
            return result > 0;
        }

        public async Task<bool> DeleteByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                    delete from genres where movie_id = @Id
                    """,
                    new { Id = id },
                    cancellationToken: cancellationToken
                )
            );

            await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                    delete from "cast" where movie_id = @Id
                    """,
                    new { Id = id },
                    cancellationToken: cancellationToken
                )
            );

            var result = await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                    delete from movies where id = @Id
                    """,
                    new { Id = id },
                    cancellationToken: cancellationToken
                )
            );

            transaction.Commit();

            return result > 0;
        }

        public async Task<bool> ExistsByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            return await connection.ExecuteScalarAsync<bool>(
                new CommandDefinition(
                    """
                    select count(*) from movies where id = @Id
                    """,
                    new { id },
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(
            CancellationToken cancellationToken = default
        )
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            var movies = await connection.QueryAsync(
                new CommandDefinition(
                    """
                    select m.*, 
                           string_agg(DISTINCT g.name, ',') as genre, 
                           string_agg(DISTINCT c.name, ',') as "cast"
                    from movies m 
                    join genres g on m.id = g.movie_id
                    join "cast" c on m.id = c.movie_id
                    group by m.id
                    """,
                    cancellationToken: cancellationToken
                )
            );

            return movies.Select(x => new Movie
            {
                Id = x.id,
                Title = x.title,
                Description = x.description,
                Duration = x.duration,
                ReleaseYear = x.releaseyear,
                Director = x.director,
                Trailer = x.trailer,
                Image = x.image,
                Genre = Enumerable.ToList(x.genre.Split(',')),
                Cast = Enumerable.ToList(x.cast.Split(',')),
            });
        }

        public async Task<Movie?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition(
                    """
                    select *
                    from movies
                    where id = @Id
                    """,
                    new { Id = id },
                    cancellationToken: cancellationToken
                )
            );

            if (movie is null)
            {
                return null;
            }

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition(
                    """
                    select name
                    from genres
                    where movie_id = @MovieId
                    """,
                    new { MovieId = id },
                    cancellationToken: cancellationToken
                )
            );

            foreach (var genre in genres)
            {
                movie.Genre.Add(genre);
            }

            var casts = await connection.QueryAsync<string>(
                new CommandDefinition(
                    """
                    select name
                    from "cast"
                    where movie_id = @MovieId
                    """,
                    new { MovieId = id },
                    cancellationToken: cancellationToken
                )
            );

            foreach (var cast in casts)
            {
                movie.Cast.Add(cast);
            }

            return movie;
        }

        public async Task<bool> UpdateAsync(
            Movie movie,
            CancellationToken cancellationToken = default
        )
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                      delete from genres where movie_id = @Id                
                    """,
                    new { id = movie.Id },
                    cancellationToken: cancellationToken
                )
            );
            await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                      delete from "cast" where movie_id = @Id                
                    """,
                    new { id = movie.Id },
                    cancellationToken: cancellationToken
                )
            );

            foreach (var genre in movie.Genre)
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        """
                        insert into genres (movie_id, name)
                        values (@MovieId, @Name)
                        """,
                        new { MovieId = movie.Id, Name = genre },
                        cancellationToken: cancellationToken
                    )
                );
            }
            foreach (var cast in movie.Cast)
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        """
                        insert into "cast" (movie_id, name)
                        values (@MovieId, @Name)
                        """,
                        new { MovieId = movie.Id, Name = cast },
                        cancellationToken: cancellationToken
                    )
                );
            }
            var result = await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                    update movies set slug = @Slug, title = @Title, description = @Description, duration = @Duration, releaseyear = @ReleaseYear, director = @Director, trailer = @Trailer, image = @Image
                    where id = @Id
                    """,
                    movie,
                    cancellationToken: cancellationToken
                )
            );

            transaction.Commit();
            return result > 0;
        }

        async Task<Movie?> IMovieRepository.GetBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default
        )
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition(
                    """
                    select *
                    from movies
                    where slug = @Slug
                    """,
                    new { Slug = slug },
                    cancellationToken: cancellationToken
                )
            );

            if (movie is null)
            {
                return null;
            }

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition(
                    """
                    select name
                    from genres
                    where movie_id = @MovieId
                    """,
                    new { MovieId = movie.Id },
                    cancellationToken: cancellationToken
                )
            );

            foreach (var genre in genres)
            {
                movie.Genre.Add(genre);
            }

            var casts = await connection.QueryAsync<string>(
                new CommandDefinition(
                    """
                    select name
                    from "cast"
                    where movie_id = @MovieId
                    """,
                    new { MovieId = movie.Id },
                    cancellationToken: cancellationToken
                )
            );

            foreach (var cast in casts)
            {
                movie.Genre.Add(cast);
            }

            return movie;
        }
    }
}
