using Microsoft.EntityFrameworkCore;
using PublisherData;
using PublisherDomain.Models;

namespace EfCoreFundamental
{
    public class Program
    {
        private  static PubContext _dbContext;
        static void Main(string[] args)
        {
            _dbContext = new PubContext();
            BulkOperationAddAuthors();
        } 

        static void BulkOperationAddAuthors()
        {
            List<Author> authors = new List<Author>();
            for(int i = 0; i < 1000; i++)
            {
                authors.Add(new Author { FirstName = $"Julie{i}", LastName = $"Lerman{i}" });
            }
           
            _dbContext.Authors.AddRange(authors);
            _dbContext.SaveChanges();
        }
        static void GetAuthors()
        {
            var authors = _dbContext.Authors.ToList();
            foreach (var auth in authors)
            {
                Console.WriteLine($"Id: {auth.AuthorId} First Name: {auth.FirstName} Last Name: {auth.LastName}");
            }
        }

        static void GetOneAuthor()
        {
            string authorName = "Julie";
            var author = _dbContext.Authors.Where(a => a.FirstName == authorName).FirstOrDefault();
            if (author != null)
            {
                Console.WriteLine($"Id: {author.AuthorId} First Name: {author.FirstName} Last Name: {author.LastName}");
            }
        }

        static void AddAuthor()
        {
            Author author = new Author { FirstName = "Julie", LastName = "Lerman" };
            _dbContext.Authors.Add(author);
            _dbContext.SaveChanges();
        }

        static void AddAndUpdateAuthor()
        {
            List<Author> alreadyExistings = new List<Author>();
            List<Author> newAdded = new List<Author>();
            Author alreadyExisting = _dbContext.Authors
                                        .Where(x => x.AuthorId == 1).FirstOrDefault(); //Julie
            alreadyExisting.FirstName = "Julie";
            alreadyExisting = _dbContext.Authors
                                .Where(x => x.AuthorId == 2).FirstOrDefault(); //Ruth
            alreadyExisting.FirstName = "Ruth";
            Author author = new Author { FirstName = "Prem", LastName = "Shankar" };
            _dbContext.Authors.AddRange(author);
            _dbContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_dbContext.ChangeTracker.DebugView.ShortView);
            _dbContext.SaveChanges();
        }

        #region Reading And Writing Some Related Data
        static void AddAuthorWithBook()
        {
            Author author = new Author { FirstName = "Julie", LastName = "Lerman" };
            author.Books.Add(new Book
            {
                Title = "Programming Entity Framework",
                PublishDate = new DateTime(2009, 1, 1)
            });
            author.Books.Add(new Book
            {
                Title = "Programming Entity Framework 2nd Ed.",
                PublishDate= new DateTime(2010, 8, 1)
            });
            _dbContext.Authors.Add(author);
            _dbContext.SaveChanges(true);
        }

        static void GetAuthorsWithBooks()
        {
            List<Author> authors = _dbContext.Authors
                                    .Include(a => a.Books)
                                    .ToList();

            foreach(Author author in authors)
            {
                Console.WriteLine($"Id: {author.AuthorId} First Name: {author.FirstName} Last Name: {author.LastName}");
                foreach(Book book in author.Books)
                {
                    Console.WriteLine($"BookId: {book.BookId} Book Title: {book.Title} Published Date {book.PublishDate}");
                }
            }
        }
        #endregion

        #region Querying with filters
        static void QueryFilters()
        {
            //string name = "Julie";
            //var authors = _dbContext.Authors.Include(a => a.Books).Where(a => a.FirstName == name).ToList();

            string filter = "L%";
            //var authors = _dbContext.Authors
            //                .Where(a => a.LastName.Contains("L")).ToList();
            var authors = _dbContext.Authors
                           .Where(a => EF.Functions.Like(a.LastName, filter)).ToList(); 
            foreach (Author author in authors)
            {
                Console.WriteLine($"Id: {author.AuthorId} First Name: {author.FirstName} Last Name: {author.LastName}");
                foreach (Book book in author.Books)
                {
                    Console.WriteLine($"BookId: {book.BookId} Book Title: {book.Title} Published Date {book.PublishDate}");
                }
            }
        }

        static void QueryFiltersUsingKey()
        {
            var author = _dbContext.Authors.Find(1);
            Console.WriteLine($"Id: {author.AuthorId} First Name: {author.FirstName} Last Name: {author.LastName}");
        }

        static void SkipAndTakeAuthors()
        {
            // Add Some More Authors
            _dbContext.Authors.Add(new Author { FirstName = "Rhoda", LastName = "Lerman" });
            _dbContext.Authors.Add(new Author { FirstName = "Don", LastName = "Jones" });
            _dbContext.Authors.Add(new Author { FirstName = "Jim", LastName = "christopher" });
            _dbContext.Authors.Add(new Author { FirstName = "Stephen", LastName = "Haunts" });
            _dbContext.SaveChanges();


            int groupSize = 2;
            for(int i = 0; i<5; i++)
            {
                var authors = _dbContext.Authors.Skip(groupSize*i).Take(groupSize).ToList();
                Console.WriteLine($"Group {i}");
                foreach(var author in authors)
                {
                    Console.WriteLine($"Id: {author.AuthorId} First Name: {author.FirstName} Last Name: {author.LastName}");
                }
            }
        }
        
        static void SortAuthors()
        {
            List<Author> authorsByLastName = _dbContext.Authors
                                                .OrderBy(a => a.LastName)
                                                .ThenBy(a => a.FirstName)
                                                .ToList();
            authorsByLastName.ForEach(author => Console.WriteLine($"Id: {author.AuthorId} First Name: {author.FirstName} Last Name: {author.LastName}"));

            List<Author> authorsDescending = _dbContext.Authors
                                            .OrderByDescending(a => a.LastName)
                                            .ThenByDescending(a => a.FirstName).ToList();

            Console.WriteLine("**Descending Last And First**");
            authorsDescending.ForEach(author => Console.WriteLine($"Id: {author.AuthorId} First Name: {author.FirstName} Last Name: {author.LastName}"));
        }
        #endregion
        # region Updating an object

        static void RetrieveAndUpdateAuthor()
        {
            Author author = _dbContext.Authors.FirstOrDefault(a => a.FirstName == "Julie" && a.LastName == "Lerman");
            if (author != null)
            {
                author.FirstName = "Julia";
                _dbContext.SaveChanges();
            }
        }

        static void RetrieveAndUpdateMultipleAuthor()
        {
            List<Author> author = _dbContext.Authors.Where(a => a.LastName == "Lerman").ToList();
            _dbContext.Authors.Add(new Author
            {
                FirstName = "Prem",
                LastName = "Shankar"
            });
            foreach (Author auth in author)
            {
                auth.LastName = "Lehrman";
            }
            Console.WriteLine("Before" + _dbContext.ChangeTracker.DebugView.ShortView);
            _dbContext.ChangeTracker.DetectChanges();
            Console.WriteLine("After: " + _dbContext.ChangeTracker.DebugView.ShortView);
            _dbContext.SaveChanges();
        }
        #endregion

        #region Updating an untracked objects
        static void CoordinatedRetrieveAndUpdateAuthor()
        {
            Author author = FindThatAuthor(1);
            if (author != null)
            {
                author.FirstName = "Julia";
                SaveThatAuthor(author);
            }
        }

        static Author FindThatAuthor(int authorId)
        {
            using var shortLivedContext = new PubContext();
            return shortLivedContext.Authors.Find(authorId);
        }

        static void SaveThatAuthor(Author author)
        {
            using var anotherShortLivedContext = new PubContext();
            anotherShortLivedContext.Authors.Update(author);
            Console.WriteLine("Before" + anotherShortLivedContext.ChangeTracker.DebugView.LongView);
            _dbContext.ChangeTracker.DetectChanges();
            Console.WriteLine("After: " + anotherShortLivedContext.ChangeTracker.DebugView.LongView);
            anotherShortLivedContext.SaveChanges();
        }
        #endregion
        #region Delete a simple object
        static void DeleteAnAuthor()
        {
            Author author = _dbContext.Authors.Find(9);
            if (author != null)
            {
                _dbContext.Authors.Remove(author);
                Console.WriteLine("Before" + _dbContext.ChangeTracker.DebugView.ShortView);
                _dbContext.ChangeTracker.DetectChanges();
                Console.WriteLine("After: " + _dbContext.ChangeTracker.DebugView.ShortView);
                _dbContext.SaveChanges();
            }
        }
        #endregion
        #region Intertacting With Related Data

        static void InsertNewAuthorWithNewBook()
        {
            Author author = new Author { FirstName = "Lynda", LastName = "Rutledge" };
            author.Books.Add(new Book
            {
                Title = "West With Giraffes",
                PublishDate = new DateTime(2021, 2, 1)
            });
            _dbContext.Authors.Add(author);
            _dbContext.SaveChanges();
        }

        static void InsertNewAuthorWith2NewBook()
        {
            Author author = new Author { FirstName = "Don", LastName = "Jones" };
            author.Books.AddRange(new List<Book>
            {
                new Book {Title = "The Neveer", PublishDate = new DateTime(2019, 12,1)},
                new Book {Title = "Alabaster", PublishDate = new DateTime(2019, 4,1)} 
            });
            _dbContext.Authors.Add(author);
            _dbContext.SaveChanges();
        }

        static void AddNewBookToExistingAuthorInMemory()
        {
            Author author = _dbContext.Authors.FirstOrDefault(a => a.LastName == "Jones");
            if (author != null)
            {
                author.Books.Add(new Book
                {
                    Title = "Wool",
                    PublishDate = new DateTime(2012, 1, 1)
                });
            }
            _dbContext.SaveChanges();
        }

        // Eager Loading Include Related Objects In query
        static void EagerLoadBooksWithAuthors()
        {
            List<Author> authorList = _dbContext.Authors
                                        .Include(a => a.Books).ToList();
            authorList.ForEach(author =>
            {
                Console.WriteLine($"{author.LastName}, ({author.Books.Count})");
                author.Books.ForEach(b => Console.WriteLine(" " + b.Title));
            });
        }

        static void EagerLoadBooksWithAuthorsWithFilter()
        {
            DateTime pubDateStart = new DateTime(2010, 1, 1);
            List<Author> authors = _dbContext.Authors
                                    .Include(a => a.Books
                                                    .Where(b => b.PublishDate >= pubDateStart)
                                                    .OrderBy(b => b.Title)).ToList();
            authors.ForEach(author =>
            {
                Console.WriteLine($"{author.LastName}, ({author.Books.Count})");
                author.Books.ForEach(b => Console.WriteLine(" " + b.Title));
            });
        }

        // Explicit Loading 
        // Explicitly request related data for the objects which is already in memory

        static void ExplicitLoadCollection()
        {
            // One to many
            var author = _dbContext.Authors.FirstOrDefault(a => a.LastName == "Howey");
            _dbContext.Entry(author).Collection(a => a.Books).Load();
            Console.WriteLine($"{author.LastName}, ({author.Books.Count})");
            author.Books.ForEach(b => Console.WriteLine(" " + b.Title));
        }

        static void ExplicitLoadReference()
        {
            var book = _dbContext.Books.FirstOrDefault(b => b.Title == "Alpha");
            _dbContext.Entry(book).Reference(b => b.Author).Query().Where(Author => Author.LastName == "Julie").Load();
        }
        // Lazy Loading
        // on-the-fly retrieval of data related to objects in memory
        static void LazyLoadBooksFromAnAuthor()
        {
            // requires lazy loading to be set up in your app
            var author = _dbContext.Authors.FirstOrDefault(a => a.LastName == "Howey");
            foreach (var book in author.Books)
            {
                Console.WriteLine(book.Title);
            }
        }

        static void FilterUsingRelatedData()
        {
            List<Author> recentAuthors = _dbContext.Authors
                                        .Where(a => a.Books.Any(b => b.PublishDate.Year >= 2015))
                                        .ToList();

        }

        static void CascadeDeleteInActionWhenTracked()
        {
            Author author = _dbContext.Authors
                            .Include(author => author.Books)
                            .FirstOrDefault(a => a.AuthorId == 1);
            _dbContext.Authors.Remove(author);

            var state = _dbContext.ChangeTracker.DebugView.ShortView;
            _dbContext.SaveChanges();
        }

        #endregion

        #region Defining and using many to many relationship

        static void ConnectExistingArtistAndCoverObjects()
        {
            Artist artistA = _dbContext.Artists.Find(1);
            Artist artistB = _dbContext.Artists.Find(2);
            Cover coverA = _dbContext.Covers.Find(1);
            coverA.Artists.Add(artistA);
            coverA.Artists.Add(artistB);
            _dbContext.SaveChanges();
        }

        static void CreateNewCoverWithExistingArtist()
        {
            Artist artistA = _dbContext.Artists.Find(1);
            Cover cover = new Cover { DesignIdeas = "Author has provided a photo", DigitalOnly = true };
            cover.Artists.Add(artistA);
            _dbContext.Covers.Add(cover);
            _dbContext.SaveChanges();
        }

        static void CreateNewCoverAndArtistTogether()
        {
            Artist newArtist = new Artist { FirstName = "Kir", LastName = "Talmage" };
            Cover newCover = new Cover { DesignIdeas = "We Like Birds", DigitalOnly = true };
            newArtist.Covers.Add(newCover);
            _dbContext.Artists.Add(newArtist);
            _dbContext.SaveChanges();
        }

        static void RetrieveAnArtistWithTheirCovers()
        {
            Artist artistWithCovers = _dbContext.Artists
                                        .Include(a => a.Covers)
                                        .FirstOrDefault(a => a.ArtistId == 1);
            if (artistWithCovers != null)
            {
                Console.WriteLine($"Artist Id: {artistWithCovers.ArtistId}, FirstName: {artistWithCovers.FirstName}, LastName: {artistWithCovers.LastName}");
                artistWithCovers.Covers.ForEach(c => Console.WriteLine($"Cover Id: {c.CoverId}, DesignIdeas: {c.DesignIdeas}, IsDigital: {c.DigitalOnly}"));
            }

        }

        static void RetrieveACoverWithItsArtists()
        {
            Cover cover = _dbContext.Covers
                            .Include(c => c.Artists)
                            .FirstOrDefault(c => c.CoverId == 1);

            if (cover != null)
            {
                Console.WriteLine($"Cover Id: {cover.CoverId}, DesignIdeas: {cover.DesignIdeas}, IsDigital: {cover.DigitalOnly}");
                cover.Artists.ForEach(a => Console.WriteLine($"Artist Id: {a.ArtistId}, FirstName: {a.FirstName}, LastName: {a.LastName}"));
            }
        }

        static void RetrieveAllArtistWithTheirCovers()
        {
            List<Artist> artistsWithTheirCovers = _dbContext.Artists
                                                    .Include(a => a.Covers).ToList();


        }

        static void RetrieveAllArtistWhoHaveCovers()
        {
            List<Artist> artistsWhoHaveCover = _dbContext.Artists
                                                .Where(a => a.Covers.Any())
                                                .ToList();

            artistsWhoHaveCover.ForEach(a => Console.WriteLine($"Artist Id: {a.ArtistId}, FirstName: {a.FirstName}, LastName: {a.LastName}"));

        }

        static void UnAssignAnArtistFromCover()
        {
            Cover coverWithArtist = _dbContext.Covers
                                    .Include(c => c.Artists.Where(a => a.ArtistId == 1))
                                    .FirstOrDefault(c => c.CoverId == 1);
            coverWithArtist.Artists.RemoveAt(0);
            _dbContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_dbContext.ChangeTracker.DebugView.ShortView);
            _dbContext.SaveChanges();
        }

        static void ReAssignACover()
        {
            Cover coverWithArtist4 = _dbContext.Covers
                                    .Include(c => c.Artists.Where(a => a.ArtistId == 4))
                                    .FirstOrDefault(c => c.CoverId == 5);

            coverWithArtist4.Artists.RemoveAt(0);
            Artist artist3 = _dbContext.Artists.Find(3);
            coverWithArtist4.Artists.Add(artist3);
            _dbContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_dbContext.ChangeTracker.DebugView.ShortView);
            _dbContext.SaveChanges();
        }

        #endregion

        #region Interacting with one to one relationship

        static void GetAllBooksWithTheirCovers()
        {
            string noCover = ": No cover Known";
            List<Book> booksAndCovers = _dbContext.Books
                                        .Include(b => b.Cover)
                                        .ToList();
            booksAndCovers.ForEach(book =>
                    Console.WriteLine($"{book.Title}" +
                    $"{book.Cover?.DesignIdeas ?? noCover}" 
                    ));
        }

        static void MultiLevelInclude()
        {
            Author authorGraph = _dbContext.Authors.AsNoTracking()
                                    .Include(a => a.Books)
                                    .ThenInclude(b => b.Cover)
                                    .ThenInclude(c => c.Artists)
                                    .FirstOrDefault(a => a.AuthorId == 1);

            Console.WriteLine($"First Name: {authorGraph.FirstName} Last Name: {authorGraph.LastName}");
            authorGraph.Books.ForEach(book =>
            {
                Console.WriteLine($"Book: {book.Title}");
                if (book.Cover != null)
                {
                    Console.WriteLine($"Design Ideas: {book.Cover.DesignIdeas}");
                    Console.WriteLine($"Artists:");
                    book.Cover.Artists.ForEach(artist => Console.WriteLine($"First Name: {artist.FirstName}, LastName: {artist.LastName}"));
                }
            });

        }

        static void NewBookAndCover()
        {
            Book book = new Book()
            {
                AuthorId = 1,
                Title = "Call Me Ishtar",
                PublishDate = new DateTime(1973, 1, 1)
            };

            book.Cover = new Cover { DesignIdeas = "Image Of Ishtar?" };

            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();
        }

        static void AddCoverToExistingBookThatHasAnUntrackedCover()
        {
            Book book = _dbContext.Books.Find(4);
            book.Cover = new Cover { DesignIdeas = "A spiral" };
            // Sql Server Throw error dure to unique foreign key constraint
            _dbContext.SaveChanges();

        }

        static void AddcoverToExistingBookWithTrackedCover()
        {
            Book book = _dbContext.Books.Include(b => b.Cover)
                .FirstOrDefault(b => b.BookId == 4);

            book.Cover = new Cover { DesignIdeas = "A Spiral" };
            _dbContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_dbContext.ChangeTracker.DebugView.ShortView);
            _dbContext.SaveChanges();
        }
        #endregion

        #region Views Stored Procedure and Raw Sql

        static void RawSqlStoredProc()
        {
            List<Author> authors = _dbContext.Authors
                                    .FromSqlRaw("AuthorsPublishedInYearRange {0}, {1}", 2010, 2015)
                                    .ToList();
        }

        static void InterpolatedSqlStoredProc()
        {
            int start = 2010;
            int end = 2015;

            List<Author> authors = _dbContext.Authors
                                    .FromSqlInterpolated($"AuthorsPublishedInYearRange {start}, {end}")
                                    .ToList();
        }

        static void GetAuthorsByArtist()
        {
            List<AuthorByArtist> authorByArtists = _dbContext.AuthorsByArtist.ToList();
            AuthorByArtist oneAuthorArtist = _dbContext.AuthorsByArtist.FirstOrDefault();
            List<AuthorByArtist> kAuthorArtists = _dbContext.AuthorsByArtist
                                                            .Where(a => a.Artist.StartsWith("K")).ToList();

            string debugView = _dbContext.ChangeTracker.DebugView.ShortView;
                                
        }

        static void DeleteCover(int coverId)
        {
            int rowCount = _dbContext.Database.ExecuteSqlRaw("DeleteCover {0}", coverId);
            Console.WriteLine(rowCount);
        }
        #endregion
    }
}