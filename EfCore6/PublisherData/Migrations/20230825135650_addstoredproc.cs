using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublisherData.Migrations
{
    /// <inheritdoc />
    public partial class addstoredproc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE PROCEDURE [dbo].[AuthorsPublishedInYearRange]

                        @yearstart int,
                        @yearend int
                    As
                    Select Distinct Authors.* from Authors
                    Left join Books On Authors.AuthorId = Books.AuthorId
                    Where YEAR(Books.PublishDate) >= @yearstart and YEAR(books.PublishDate) <= @yearend");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE [dbo].[AuthorsPublishedInYearRange]");
        }
    }
}
