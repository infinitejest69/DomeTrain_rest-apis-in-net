using System.Text.RegularExpressions;

namespace Movies.Application.Models
{
    public partial class Movie
    {
        public required Guid Id { get; init; }
        public required string Title { get; set; }

        public string Slug => GenerateSlug();

        private string GenerateSlug()
        {
            var sluggedTitle = SlugRegex().Replace(Title, string.Empty).ToLowerInvariant().Replace(" ", "-");
            return $"{sluggedTitle}-{ReleaseYear}";
        }

        public float? Rating { get; set; }
        public int? UserRating { get; set; }
        public required List<string> Genres { get; init; } = new();

        public string Description { get; set; }
        public int Duration { get; set; }
        public required int ReleaseYear { get; set; }
        public string Director { get; set; }
        public string Trailer { get; set; }
        public required string Image { get; set; }

        [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 10)]
        private static partial Regex SlugRegex();
    }
}