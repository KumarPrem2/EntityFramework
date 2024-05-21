using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublisherData.Migrations
{
    /// <inheritdoc />
    public partial class addview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW AuthorsByArtist
                                    As
                                    Select Artists.FirstName + ' ' + Artists.LastName as Artist,
	                                        Authors.FirstName + ' ' + Authors.LastName as Author
                                    FROM ARTISTS LEFT JOIN
                                    ArtistCover ON Artists.ArtistId = ArtistCover.ArtistsArtistId 
                                    Left join Covers on ArtistCover.coversCoverId = Covers.CoverId left join
                                    Books on Books.bookId = Covers.BookId left join
                                    authors on books.AuthorId = Authors.AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Drop View AuthorsByArtist");
        }
    }
}
