using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project123.Migrations
{
    public partial class editShipmentLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShipmentLocation",
                table: "ShipmentLocation");

            migrationBuilder.RenameTable(
                name: "ShipmentLocation",
                newName: "ShipmentLocationModel");

            migrationBuilder.RenameColumn(
                name: "StorageName",
                table: "ShipmentLocationModel",
                newName: "ShipmentItemText");

            migrationBuilder.RenameColumn(
                name: "StorageID",
                table: "ShipmentLocationModel",
                newName: "ShipmentItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShipmentLocationModel",
                table: "ShipmentLocationModel",
                column: "ShipmentItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShipmentLocationModel",
                table: "ShipmentLocationModel");

            migrationBuilder.RenameTable(
                name: "ShipmentLocationModel",
                newName: "ShipmentLocation");

            migrationBuilder.RenameColumn(
                name: "ShipmentItemText",
                table: "ShipmentLocation",
                newName: "StorageName");

            migrationBuilder.RenameColumn(
                name: "ShipmentItemID",
                table: "ShipmentLocation",
                newName: "StorageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShipmentLocation",
                table: "ShipmentLocation",
                column: "StorageID");
        }
    }
}
