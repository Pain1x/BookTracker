using BookTracker.DAL.Entities.Authors;
using BookTracker.DAL.Entities.Books;
using BookTracker.DAL.Entities.Genres;
using BookTracker.DAL.Entities.Languages;
using BookTracker.DAL.Entities.Translations;

using Microsoft.EntityFrameworkCore;

namespace BookTracker.DAL.DBContexts
{
	public class BooksDbContext(DbContextOptions<BooksDbContext> options) : DbContext(options)
	{
		#region DBSets

		public DbSet<Book> Books { get; set; }

		public DbSet<Author> Authors { get; set; }

		public DbSet<Genre> Genres { get; set; }

		public DbSet<Language> Languages { get; set; }

		public DbSet<BookTranslation> BookTranslations { get; set; }

		public DbSet<AuthorTranslation> AuthorTranslations { get; set; }

		public DbSet<GenreTranslation> GenreTranslations { get; set; }

		#endregion

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			#region Authors

			// Author configuration
			modelBuilder.Entity<Author>()
				.HasKey(a => a.AuthorPk);

			modelBuilder.Entity<Author>()
				.HasIndex(a => a.Name)
				.IsUnique();

			// Genre configuration
			modelBuilder.Entity<Genre>()
				.HasKey(g => g.GenrePk);

			modelBuilder.Entity<Genre>()
				.HasIndex(g => g.Name)
				.IsUnique();

			#endregion

			#region Books

			// Book configuration
			modelBuilder.Entity<Book>()
				.HasKey(b => b.BookPk);

			modelBuilder.Entity<Book>()
				.HasOne(b => b.Author)
				.WithMany() // .WithMany(a => a.Books) if you add collection
				.HasForeignKey(b => b.AuthorPk)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Book>()
				.HasOne(b => b.Genre)
				.WithMany() // .WithMany(g => g.Books) if you add collection
				.HasForeignKey(b => b.GenrePk)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Book>()
			.Property(e => e.DateRead)
			.HasColumnType("timestamptz");

			#endregion

			#region Languages

			modelBuilder.Entity<Language>()
				.HasKey(l => l.LanguagePk);

			#endregion

			#region Translations

			// BookTranslation
			modelBuilder.Entity<BookTranslation>()
				.HasKey(bt => bt.BookTranslationPk);

			modelBuilder.Entity<BookTranslation>()
				.HasIndex(bt => new { bt.BookPk, bt.LanguagePk })
				.IsUnique();

			modelBuilder.Entity<BookTranslation>()
				.HasOne(bt => bt.Book)
				.WithMany(b => b.Translations)
				.HasForeignKey(bt => bt.BookPk)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<BookTranslation>()
				.HasOne(bt => bt.Language)
				.WithMany()
				.HasForeignKey(bt => bt.LanguagePk)
				.OnDelete(DeleteBehavior.NoAction);

			// AuthorTranslation
			modelBuilder.Entity<AuthorTranslation>()
				.HasKey(at => at.AuthorTranslationPk);

			modelBuilder.Entity<AuthorTranslation>()
				.HasIndex(at => new { at.AuthorPk, at.LanguagePk })
				.IsUnique();

			modelBuilder.Entity<AuthorTranslation>()
				.HasOne(at => at.Author)
				.WithMany(a => a.Translations)
				.HasForeignKey(at => at.AuthorPk)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<AuthorTranslation>()
				.HasOne(at => at.Language)
				.WithMany()
				.HasForeignKey(at => at.LanguagePk)
				.OnDelete(DeleteBehavior.NoAction);

			// GenreTranslation
			modelBuilder.Entity<GenreTranslation>()
				.HasKey(gt => gt.GenreTranslationPk);

			modelBuilder.Entity<GenreTranslation>()
				.HasIndex(gt => new { gt.GenrePk, gt.LanguagePk })
				.IsUnique();

			modelBuilder.Entity<GenreTranslation>()
				.HasOne(gt => gt.Genre)
				.WithMany(g => g.Translations)
				.HasForeignKey(gt => gt.GenrePk)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<GenreTranslation>()
				.HasOne(gt => gt.Language)
				.WithMany()
				.HasForeignKey(gt => gt.LanguagePk)
				.OnDelete(DeleteBehavior.NoAction);

			#endregion
		}
	}
}