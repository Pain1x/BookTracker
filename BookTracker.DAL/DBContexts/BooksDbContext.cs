using BookTracker.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.DAL.DBContexts
{
    public class BooksDbContext : DbContext
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=192.168.0.135:5432;Database=Books;Username=postgres;Password=admin1;IncludeErrorDetail=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Books

            // Book configuration
            modelBuilder.Entity<Book>()
                .HasKey(b => b.BookPK);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany() // .WithMany(a => a.Books) if you add collection
                .HasForeignKey(b => b.AuthorPK)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany() // .WithMany(g => g.Books) if you add collection
                .HasForeignKey(b => b.GenrePK)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Book>()
            .Property(e => e.DateRead)
            .HasColumnType("timestamptz");

            #endregion

            // Author configuration
            modelBuilder.Entity<Author>()
                .HasKey(a => a.AuthorPK);

            modelBuilder.Entity<Author>()
                .HasIndex(a => a.Name)
                .IsUnique();

            // Genre configuration
            modelBuilder.Entity<Genre>()
                .HasKey(g => g.GenrePK);

            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique();
        }
    }
}