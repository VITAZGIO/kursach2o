using FurnitureStore.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureStore.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260531193000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Categories", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                FullName = table.Column<string>(type: "TEXT", nullable: false),
                Phone = table.Column<string>(type: "TEXT", nullable: true),
                Email = table.Column<string>(type: "TEXT", nullable: true),
                Address = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Customers", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Manufacturers",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Country = table.Column<string>(type: "TEXT", nullable: true),
                Phone = table.Column<string>(type: "TEXT", nullable: true),
                Email = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Manufacturers", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Orders",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                Status = table.Column<string>(type: "TEXT", nullable: false),
                TotalAmount = table.Column<double>(type: "REAL", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Orders", x => x.Id);
                table.ForeignKey(
                    name: "FK_Orders_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "FurnitureItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Article = table.Column<string>(type: "TEXT", nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: true),
                Material = table.Column<string>(type: "TEXT", nullable: true),
                Color = table.Column<string>(type: "TEXT", nullable: true),
                Size = table.Column<string>(type: "TEXT", nullable: true),
                Price = table.Column<double>(type: "REAL", nullable: false),
                StockQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                ManufacturerId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FurnitureItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_FurnitureItems_Categories_CategoryId",
                    column: x => x.CategoryId,
                    principalTable: "Categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_FurnitureItems_Manufacturers_ManufacturerId",
                    column: x => x.ManufacturerId,
                    principalTable: "Manufacturers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "OrderItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                FurnitureItemId = table.Column<int>(type: "INTEGER", nullable: false),
                Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                UnitPrice = table.Column<double>(type: "REAL", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_OrderItems_FurnitureItems_FurnitureItemId",
                    column: x => x.FurnitureItemId,
                    principalTable: "FurnitureItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_OrderItems_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_FurnitureItems_Article", table: "FurnitureItems", column: "Article", unique: true);
        migrationBuilder.CreateIndex(name: "IX_FurnitureItems_CategoryId", table: "FurnitureItems", column: "CategoryId");
        migrationBuilder.CreateIndex(name: "IX_FurnitureItems_ManufacturerId", table: "FurnitureItems", column: "ManufacturerId");
        migrationBuilder.CreateIndex(name: "IX_OrderItems_FurnitureItemId", table: "OrderItems", column: "FurnitureItemId");
        migrationBuilder.CreateIndex(name: "IX_OrderItems_OrderId", table: "OrderItems", column: "OrderId");
        migrationBuilder.CreateIndex(name: "IX_Orders_CustomerId", table: "Orders", column: "CustomerId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "OrderItems");
        migrationBuilder.DropTable(name: "FurnitureItems");
        migrationBuilder.DropTable(name: "Orders");
        migrationBuilder.DropTable(name: "Categories");
        migrationBuilder.DropTable(name: "Manufacturers");
        migrationBuilder.DropTable(name: "Customers");
    }
}
