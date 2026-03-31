using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slotly.Migrations
{
    /// <inheritdoc />
    public partial class FixWorkingHoursAndAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Appointments");
        }
    }
}
