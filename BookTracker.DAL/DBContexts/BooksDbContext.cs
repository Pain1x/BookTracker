using BookTracker.DAL.Entities;

using Microsoft.EntityFrameworkCore;

namespace BookTracker.DAL.DBContexts
{
	public class BooksDbContext : DbContext
	{
		public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options) { }

		public DbSet<Book> Books { get; set; }
		public DbSet<Author> Authors { get; set; }
		public DbSet<Genre> Genres { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
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
		}
	}
}