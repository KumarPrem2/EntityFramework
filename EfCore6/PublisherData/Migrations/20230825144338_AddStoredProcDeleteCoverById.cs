using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublisherData.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcDeleteCoverById : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE DeleteCover
				                            @coverid int
                                    As
                                    Delete from covers where CoverId = @coverid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop Procedure DeleteCover");
        }
    }
}
