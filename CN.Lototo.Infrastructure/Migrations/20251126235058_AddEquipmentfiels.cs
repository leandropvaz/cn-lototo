using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CN.Lototo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentfiels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FactoryName",
                table: "Equipamento",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsolationDeviceDescription",
                table: "Equipamento",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsolationDeviceLocation",
                table: "Equipamento",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsolationDeviceTag",
                table: "Equipamento",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LockoutType",
                table: "Equipamento",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionInfo",
                table: "Equipamento",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FactoryName",
                table: "Equipamento");

            migrationBuilder.DropColumn(
                name: "IsolationDeviceDescription",
                table: "Equipamento");

            migrationBuilder.DropColumn(
                name: "IsolationDeviceLocation",
                table: "Equipamento");

            migrationBuilder.DropColumn(
                name: "IsolationDeviceTag",
                table: "Equipamento");

            migrationBuilder.DropColumn(
                name: "LockoutType",
                table: "Equipamento");

            migrationBuilder.DropColumn(
                name: "RevisionInfo",
                table: "Equipamento");
        }
    }
}
