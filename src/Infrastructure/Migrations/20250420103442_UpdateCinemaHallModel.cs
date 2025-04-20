using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCinemaHallModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "capacity",
                table: "cinema_halls");

            migrationBuilder.AddColumn<string>(
                name: "seats_per_row",
                table: "cinema_halls",
                type: "json",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "seats_per_row",
                table: "cinema_halls");

            migrationBuilder.AddColumn<int>(
                name: "capacity",
                table: "cinema_halls",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
