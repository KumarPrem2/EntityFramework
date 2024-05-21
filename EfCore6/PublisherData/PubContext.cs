using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PublisherDomain.Models;
using System.Diagnostics;

namespace PublisherData
{
    public class PubContext:DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Cover> Covers { get; set; }
        public DbSet<AuthorByArtist> AuthorsByArtist { get; set; }
       

        public PubContext(DbContextOptions<PubContext> options)
            : base(options)
        {

        }

        public PubContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                "data source=CHGTUH86825\\MSSQLSERVER2019;initial catalog=PublisherDatabase;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework; Trust Server Certificate = true;",
                o =>
                {
                    o.MinBatchSize(1);
                    o.MaxBatchSize(1000);
                }).LogTo(log => Debug.WriteLine(log), new[]
                {
                    DbLoggerCategory.Database.Command.Name
                }, LogLevel.Information)
                .EnableSensitiveDataLogging();
            }  
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AuthorByArtist>()
                        .HasNoKey()
                        .ToView(nameof(AuthorsByArtist));

            modelBuilder.Entity<Author>().HasData(
                new Author { AuthorId = 1, FirstName = "Julie", LastName = "Lerman" }
                );

            List<Author> authors = new List<Author>()
            {
                new Author { AuthorId = 2, FirstName = "Ruth", LastName = "Ozeki"},
                new Author { AuthorId = 3, FirstName = "Sofia", LastName = "Segovia"},
                new Author { AuthorId = 4, FirstName = "Ursula K.", LastName = "LeGuin"},
                new Author {AuthorId = 5, FirstName = "Hugh", LastName = "Howey"},
                new Author {AuthorId = 6, FirstName = "Isabelle", LastName = "Allende"}
            };
            modelBuilder.Entity<Author>().HasData(authors);

            List<Book> books = new List<Book>()
            {
                new Book {BookId = 1,  AuthorId = 1, Title = "In God's Ear", PublishDate = new DateTime(1989,3, 1)},
                new Book {BookId = 2, AuthorId = 2, Title = "A Tale For The Time Being", PublishDate = new DateTime(2013, 12, 31)},
                new Book {BookId = 3, AuthorId = 3, Title = "The Left Hand of Darkness", PublishDate = new DateTime(1969,3, 1)}
            };
            modelBuilder.Entity<Book>()
                .HasData(books);

            // Map the foreigh key as not required
            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId)
                .IsRequired(false);

            List<Artist> artists = new List<Artist>()
            {
                new Artist {ArtistId = 1,FirstName = "Pablo", LastName = "Picaso"},
                new Artist {ArtistId = 2,FirstName = "Dee", LastName = "Bell"},
                new Artist {ArtistId=3,FirstName = "Katharine", LastName = "Kuharic"}
            };
            modelBuilder.Entity<Artist>()
                .HasData(artists);

            List<Cover> covers = new List<Cover>()
            {
                new Cover {CoverId = 1, BookId = 3, DesignIdeas = "How About a left hands in the Dark?", DigitalOnly = false},
                new Cover {CoverId = 2, BookId = 2, DesignIdeas = "Should we put a clock?", DigitalOnly = true},
                new Cover{CoverId = 3, BookId = 1, DesignIdeas = "A big ear in the clouds?", DigitalOnly= false},
            };
            modelBuilder.Entity<Cover>()
                .HasData(covers);

            // mapping skip navigation with payload
            modelBuilder.Entity<Artist>()
                .HasMany(a => a.Covers)
                .WithMany(b => b.Artists)
                .UsingEntity<CoverAssignment>(
                    join => join
                        .HasOne<Cover>()
                        .WithMany()
                        .HasForeignKey(ca => ca.CoverId),
                    join => join
                        .HasOne<Artist>()
                        .WithMany()
                        .HasForeignKey(ca => ca.ArtistId));

            modelBuilder.Entity<CoverAssignment>()
                        .Property(ca => ca.DateCreated).HasDefaultValueSql("GetDate()");
            modelBuilder.Entity<CoverAssignment>()
                        .Property(ca => ca.CoverId).HasColumnName("CoversCoverId");
            modelBuilder.Entity<CoverAssignment>()
                        .Property(ca => ca.ArtistId).HasColumnName("ArtistsArtistId");

        }
    }
}