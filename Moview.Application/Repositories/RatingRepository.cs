using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        int result = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                delete from ratings
                where movie_id = @movieId
                  and userid = @userId
                """, new { movieId, userId },
                cancellationToken: cancellationToken
            )
        );
        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        float? movies = await connection.QuerySingleOrDefaultAsync<float?>(
            new CommandDefinition(
                """
                select round(avg(r.rating), 1) from ratings r
                where movie_id = @movieId
                """, new { movieId },
                cancellationToken: cancellationToken
            )
        );
        return movies;
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        (float?, int?) movies = await connection.QuerySingleOrDefaultAsync<(float?, int?)>(
            new CommandDefinition(
                """
                select round(avg(rating), 1), 
                       (select rating 
                        from ratings 
                        where movie_id = @movieId 
                          and userid = @userId
                        limit 1) 
                from ratings
                where movieid = @movieId                                       
                """, new { movieId, userId },
                cancellationToken: cancellationToken));
        return movies;
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid? userId = null, CancellationToken cancellationToken = default)
    {
        using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        IEnumerable<MovieRating> movies = await connection.QueryAsync<MovieRating>(
            new CommandDefinition(
                """
                select r.rating, r.movie_id, m.slug
                from ratings r
                inner join movies m on r.movie_id = m.id
                where r.userid = @userId
                """, new { userId },
                cancellationToken: cancellationToken
            )
        );
        return movies;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cancellationToken = default)
    {
        using System.Data.IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        int result = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                insert into ratings(userid, movie_id, rating) 
                values (@userId, @movieId, @rating)
                on conflict (userid, movie_id) do update 
                    set rating = @rating
                """, new { movieId, userId, rating },
                cancellationToken: cancellationToken
            )
        );
        return result > 0;
    }
}