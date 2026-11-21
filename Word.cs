using Microsoft.EntityFrameworkCore;

namespace LanguageHelper.Models 
{
    public class Word
    {
          public int Id { get; set; }
          public string? Text { get; set; }
          public string? Translation { get; set; }
          public string? Language { get; set; }
          public int? FrequencyRank { get; set; }
          public bool Starred { get; set; }
          public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
          public Card? Card { get; set; }
          public List<Capture> Captures { get; set; } = new();

    }
       public class Card
    {
        public int Id { get; set; }
        public int WordId { get; set; }
        public Word? Word { get; set; }
        public double EaseFactor { get; set; } = 2.5;
        public int IntervalDays { get; set; } = 0;
        public int Repetitions { get; set; } = 0;
        public DateTime DueDate { get; set; } = DateTime.UtcNow;
    }

    public class Capture
    {
        public int Id { get; set; }
        public int WordId { get; set; }
        public Word? Word { get; set; }
        public string? Sentence { get; set; }
        public string? VideoUrl { get; set; }
        public int TimestampSeconds { get; set; }
        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
    }

    public record CreateWordRequest(string Text, string? Translation, string? Language, string? Sentence);


    class WordDb : DbContext
    {
        public WordDb(DbContextOptions options) : base(options) { }
        public DbSet<Word> Words { get; set; } = null!;
        public DbSet<Card> Cards { get; set; } = null!;
        public DbSet<Capture> Captures { get; set; } = null!;
    }
}