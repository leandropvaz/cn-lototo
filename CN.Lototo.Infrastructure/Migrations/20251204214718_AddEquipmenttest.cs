using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CN.Lototo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmenttest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Test",
                table: "Equipamento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZeroEnergyVerification",
                table: "Equipamento",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Test",
                table: "Equipamento");

            migrationBuilder.DropColumn(
                name: "ZeroEnergyVerification",
                table: "Equipamento");
        }
    }
}
