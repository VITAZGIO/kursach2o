using FurnitureStore.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureStore.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260531200000_AddAuthCartOrders")]
public partial class AddAuthCartOrders : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AppUsers",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Login = table.Column<string>(type: "TEXT", nullable: false),
                Password = table.Column<string>(type: "TEXT", nullable: false),
                Role = table.Column<string>(type: "TEXT", nullable: false),
                FullName = table.Column<string>(type: "TEXT", nullable: false),
                Phone = table.Column<string>(type: "TEXT", nullable: true),
                Email = table.Column<string>(type: "TEXT", nullable: true),
                Address = table.Column<string>(type: "TEXT", nullable: true),
                CustomerId = table.Column<int>(type: "INTEGER", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppUsers", x => x.Id);
                table.ForeignKey(
                    name: "FK_AppUsers_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "CartItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                AppUserId = table.Column<int>(type: "INTEGER", nullable: false),
                FurnitureItemId = table.Column<int>(type: "INTEGER", nullable: false),
                Quantity = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CartItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_CartItems_AppUsers_AppUserId",
                    column: x => x.AppUserId,
                    principalTable: "AppUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CartItems_FurnitureItems_FurnitureItemId",
                    column: x => x.FurnitureItemId,
                    principalTable: "FurnitureItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(name: "IX_AppUsers_CustomerId", table: "AppUsers", column: "CustomerId");
        migrationBuilder.CreateIndex(name: "IX_AppUsers_Login", table: "AppUsers", column: "Login", unique: true);
        migrationBuilder.CreateIndex(name: "IX_CartItems_AppUserId", table: "CartItems", column: "AppUserId");
        migrationBuilder.CreateIndex(name: "IX_CartItems_FurnitureItemId", table: "CartItems", column: "FurnitureItemId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CartItems");
        migrationBuilder.DropTable(name: "AppUsers");
    }
}
