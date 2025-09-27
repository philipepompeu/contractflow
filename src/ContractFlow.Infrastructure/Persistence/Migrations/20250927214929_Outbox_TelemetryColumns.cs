using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Outbox_TelemetryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "correlation_id",
                table: "outbox_messages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "correlation_id",
                table: "outbox_messages");
        }
    }
}
