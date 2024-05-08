using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project123.Migrations
{
    public partial class addShipmentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShipmentLocation",
                table: "ShipmentLocation");

        
            migrationBuilder.RenameColumn(
                name: "StorageName",
                table: "ShipmentLocation",
                newName: "ShipmentItemText");

            migrationBuilder.RenameColumn(
                name: "StorageID",
                table: "ShipmentLocation",
                newName: "ShipmentItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShipmentLocationModel",
                table: "ShipmentLocation",
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
