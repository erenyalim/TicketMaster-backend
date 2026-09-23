using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IsDeletedFilterAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketTypes_EventId_Name",
                table: "TicketTypes");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_EventId_Name",
                table: "TicketTypes",
                columns: new[] { "EventId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketTypes_EventId_Name",
                table: "TicketTypes");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypes_EventId_Name",
                table: "TicketTypes",
                columns: new[] { "EventId", "Name" },
                unique: true);
        }
    }
}
