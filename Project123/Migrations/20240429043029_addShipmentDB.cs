using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project123.Migrations
{
    public partial class addShipmentDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shipment",
                columns: table => new
                {
                    ShipmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(300)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    Storage = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    ShipmentStatus = table.Column<int>(type: "int", nullable: false),
                    ShipDate = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    ShipDateFR = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    ShipDateTO = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment", x => x.ShipmentId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shipment");
        }
    }
}
