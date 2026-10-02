using System;
using BookTracker.Common.Enums;

namespace BookTracker.BLL.Models
{
    public class BookModel
    {
        public string? DateReadString => DateRead?.UtcDateTime.ToString("yyyy-MM-dd");

        public Guid BookPk { get; set; }

        public string Title { get; set; } = "";

        public DateTimeOffset? DateRead { get; set; }

        public int Rating { get; set; }

        public string Notes { get; set; } = "";
        
        public required AuthorModel Author { get; set; }
        
        public required GenreModel Genre { get; set; }

        public Languages TargetLanguage { get; set; }
    }
}