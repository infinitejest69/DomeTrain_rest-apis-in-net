﻿namespace Movies.Contracts.Requests
{
    public class CreateMovieRequest
    {
        public required string Title { get; init; }
        public string Description { get; init; }
        public required IEnumerable<string> Genre { get; init; } = [];
        public int Duration { get; init; }
        public required int ReleaseYear { get; init; }
        public string Director { get; init; }
        public required IEnumerable<string> Cast { get; init; }
        public string Trailer { get; init; }
        public required string Image { get; init; }
    }
}
