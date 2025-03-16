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
            using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            using System.Data.IDbTransaction transaction = connection.BeginTransaction();

            int result = await connection.ExecuteAsync(
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
                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(new CommandDefinition("""
                    insert into genres (movieId, name) 
                    values (@MovieId, @Name)
                    """, new { MovieId = movie.Id, Name = genre }, cancellationToken: cancellationToken));
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
            using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            using System.Data.IDbTransaction transaction = connection.BeginTransaction();

            _ = await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                    delete from genres where movie_id = @Id
                    """,
                    new { Id = id },
                    cancellationToken: cancellationToken
                )
            );

            _ = await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                    delete from "cast" where movie_id = @Id
                    """,
                    new { Id = id },
                    cancellationToken: cancellationToken
                )
            );

            int result = await connection.ExecuteAsync(
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
            Guid id, Guid? userId = default,
            CancellationToken cancellationToken = default
        )
        {
            using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
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

        public async Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default,
            CancellationToken cancellationToken = default
        )
        {
            using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            IEnumerable<dynamic> movies = await connection.QueryAsync(
    new CommandDefinition(
        """
        select m.*, 
               string_agg(distinct g.name, ',') as genres , 
               round(avg(r.rating), 1) as rating, 
               myr.rating as userrating
        from movies m 
        left join genres g on m.id = g.movie_id
        left join ratings r on m.id = r.movie_id
        left join ratings myr on m.id = myr.movie_id
            and myr.userid = @userId
        group by id, userrating
        """, new { userId },
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
                Rating = (float?)x.rating,
                UserRating = (int?)x.userrating,
                Genres = Enumerable.ToList(x.genres.Split(','))
            });
        }

        public async Task<Movie?> GetByIdAsync(
            Guid id, Guid? userId = default,
            CancellationToken cancellationToken = default
        )
        {
            using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            Movie? movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition(
    """
    select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating 
    from movies m
    left join ratings r on m.id = r.movie_id
    left join ratings myr on m.id = myr.movie_id
        and myr.userid = @userId
    where id = @id
    group by id, userrating
    """,
    new { Id = id, userId },
    cancellationToken: cancellationToken
)
            );
            if (movie is null)
            {
                return null;
            }

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition("""
            select name from genres where movie_id = @id 
            """, new { id }, cancellationToken: cancellationToken));

            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }

            return movie;
        }

        public async Task<bool> UpdateAsync(
            Movie movie, Guid? userId = default,
            CancellationToken cancellationToken = default
        )
        {
            using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            using System.Data.IDbTransaction transaction = connection.BeginTransaction();

            _ = await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                      delete from genres where movie_id = @Id                
                    """,
                    new { id = movie.Id },
                    cancellationToken: cancellationToken
                )
            );
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into genres (movieId, name) 
                    values (@MovieId, @Name)
                    """, new { MovieId = movie.Id, Name = genre }, cancellationToken: cancellationToken));
            }

            int result = await connection.ExecuteAsync(
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
            Guid? userId = default,
            CancellationToken cancellationToken = default
        )
        {
            using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            Movie? movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition(
                    """
                    select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
                    from movies m
                    left join ratings r on m.id = r.movieid
                    left join ratings myr on m.id = myr.movieid
                        and myr.userid = @userId
                    where slug = @slug
                    group by id, userrating
                    """,
                    new { Slug = slug, userId },
                    cancellationToken: cancellationToken
                )
            );
            if (movie is null)
            {
                return null;
            }

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition("""
            select name from genres where movieid = @id 
            """, new { id = movie.Id }, cancellationToken: cancellationToken));

            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }


            return movie;
        }
    }
}