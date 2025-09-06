using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContractInheritance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                table: "contracts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "contract_type",
                table: "contracts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "supplier_id",
                table: "contracts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_contracts_supplier",
                table: "contracts",
                column: "supplier_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_contracts_supplier",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "contract_type",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "supplier_id",
                table: "contracts");

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                table: "contracts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
