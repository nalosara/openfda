using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace openfda.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAdverseEventDrugId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdverseEvents_Drugs_DrugId",
                table: "AdverseEvents");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EventDate",
                table: "AdverseEvents",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<Guid>(
                name: "DrugId",
                table: "AdverseEvents",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_AdverseEvents_Drugs_DrugId",
                table: "AdverseEvents",
                column: "DrugId",
                principalTable: "Drugs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdverseEvents_Drugs_DrugId",
                table: "AdverseEvents");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EventDate",
                table: "AdverseEvents",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DrugId",
                table: "AdverseEvents",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_AdverseEvents_Drugs_DrugId",
                table: "AdverseEvents",
                column: "DrugId",
                principalTable: "Drugs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
