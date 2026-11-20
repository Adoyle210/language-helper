using Microsoft.EntityFrameworkCore;

namespace LanguageHelper.Models 
{
    public class Word
    {
          public int Id { get; set; }
          public string? Text { get; set; }
          public string? Translation { get; set; }
          public bool Stared { get; set; }
    }
    class WordDb : DbContext
    {
        public WordDb(DbContextOptions options) : base(options) { }
        public DbSet<Word> Words { get; set; } = null!;
    }
}