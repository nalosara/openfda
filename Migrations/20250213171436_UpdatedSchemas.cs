using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace openfda.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenericName",
                table: "Drugs");

            migrationBuilder.UpdateData(
                table: "OpenFda",
                keyColumn: "BrandName",
                keyValue: null,
                column: "BrandName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "BrandName",
                table: "OpenFda",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerName",
                table: "OpenFda",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManufacturerName",
                table: "OpenFda");

            migrationBuilder.AlterColumn<string>(
                name: "BrandName",
                table: "OpenFda",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "GenericName",
                table: "Drugs",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
