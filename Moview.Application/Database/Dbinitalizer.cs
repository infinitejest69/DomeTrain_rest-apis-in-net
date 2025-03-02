using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Database
{
    public class Dbinitalizer
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public Dbinitalizer(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task InitializeAsync()
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            await connection.ExecuteAsync("""
                CREATE TABLE IF NOT EXISTS movies               
                (
                id UUID PRIMARY KEY,
                slug TEXT NOT NULL,
                title TEXT NOT NULL,
                description TEXT,
                duration INT,
                releaseyear INT,
                director TEXT,               
                trailer TEXT,
                image TEXT 
                )
                """);

            await connection.ExecuteAsync("""
                CREATE unique INDEX concurrently IF NOT EXISTS movies_slug_idx 
                ON movies
                using btree(slug)
                """);

            await connection.ExecuteAsync("""
                CREATE table if not exists genres(
                movie_id UUID REFERENCES movies(id),
                name TEXT not null
                )                
                """);

            await connection.ExecuteAsync("""
                CREATE table if not exists "cast"(
                movie_id UUID REFERENCES movies(id),
                name TEXT not null
                )                
                """);

        }
    }
}
