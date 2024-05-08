using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project123.Migrations
{
    public partial class addShipmentLocation1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShipmentLocation",
                columns: table => new
                {
                    StorageID = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    StorageName = table.Column<string>(type: "nvarchar(300)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentLocation", x => x.StorageID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentLocation");
        }
    }
}
