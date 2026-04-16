using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slotly.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_StaffServices_StaffServiceId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "StaffServiceId",
                table: "Appointments",
                newName: "StaffId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_StaffServiceId",
                table: "Appointments",
                newName: "IX_Appointments_StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Staffs_StaffId",
                table: "Appointments",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Staffs_StaffId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "Appointments",
                newName: "StaffServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_StaffId",
                table: "Appointments",
                newName: "IX_Appointments_StaffServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_StaffServices_StaffServiceId",
                table: "Appointments",
                column: "StaffServiceId",
                principalTable: "StaffServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
