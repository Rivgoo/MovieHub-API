using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBookingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "seat_id",
                table: "bookings",
                newName: "seat_number");

            migrationBuilder.AddColumn<int>(
                name: "row_number",
                table: "bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "row_number",
                table: "bookings");

            migrationBuilder.RenameColumn(
                name: "seat_number",
                table: "bookings",
                newName: "seat_id");
        }
    }
}
