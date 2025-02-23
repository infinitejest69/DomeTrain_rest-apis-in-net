using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Models
{
    public class Movie
    {
        public required Guid Id { get; init; }
        public required string Title { get; set; }
        public string Description { get; set; }
        public required List<string> Genre { get; set; } = [];
        public int Duration { get; set; }
        public required DateOnly ReleaseDate { get; set; }
        public string Director { get; set; }
        public required List<string> Cast { get; set; } = [];
        public string Trailer { get; set; }
        public required string Image { get; set; }
    }
}
