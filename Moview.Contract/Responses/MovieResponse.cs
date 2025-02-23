namespace Movies.Contracts.Responses
{
    public class MovieResponse
    {
        public required Guid Id { get; init; }
        public required string Title { get; init; }
        public string Description { get; init; }
        public required IEnumerable<string> Genre { get; init; } = [];
        public int Duration { get; init; }
        public required DateOnly ReleaseDate { get; init; }
        public string Director { get; init; }
        public required IEnumerable<string> Cast { get; init; } = [];
        public string Trailer { get; init; }
        public required string Image { get; init; }

    }
}
