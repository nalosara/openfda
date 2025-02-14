using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace openfda.Migrations
{
    /// <inheritdoc />
    public partial class AdverseEventGenderAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AdverseEvents",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AdverseEvents");
        }
    }
}
