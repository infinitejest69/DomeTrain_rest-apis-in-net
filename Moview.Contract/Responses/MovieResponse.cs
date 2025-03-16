namespace Movies.Contracts.Responses
{
    public class MovieResponse
    {
        public required Guid Id { get; init; }
        public required string Slug { get; init; }
        public required string Title { get; init; }
        public string Description { get; init; }
        public int Duration { get; init; }
        public int? UserRating { get; init; }
        public float? Rating { get; init; }
        public required int ReleaseYear { get; init; }
        public string Director { get; init; }
        public string Trailer { get; init; }
        public required string Image { get; init; }
        public required IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();

    }
}
