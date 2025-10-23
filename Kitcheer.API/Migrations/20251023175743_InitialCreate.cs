using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kitcheer.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Barcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProductType = table.Column<string>(type: "text", nullable: false),
                    MinimumQuantity = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                    DefaultUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProductData = table.Column<string>(type: "jsonb", nullable: true),
                    DeleteFl = table.Column<bool>(type: "boolean", nullable: false),
                    RekordChange = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ListData = table.Column<string>(type: "jsonb", nullable: true),
                    DeleteFl = table.Column<bool>(type: "boolean", nullable: false),
                    RekordChange = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    AdditionalData = table.Column<string>(type: "jsonb", nullable: true),
                    DeleteFl = table.Column<bool>(type: "boolean", nullable: false),
                    RekordChange = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingListItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShoppingListId = table.Column<int>(type: "integer", nullable: false),
                    ProductTemplateId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsPurchased = table.Column<bool>(type: "boolean", nullable: false),
                    ItemData = table.Column<string>(type: "jsonb", nullable: true),
                    DeleteFl = table.Column<bool>(type: "boolean", nullable: false),
                    RekordChange = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingListItems_ProductTemplates_ProductTemplateId",
                        column: x => x.ProductTemplateId,
                        principalTable: "ProductTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShoppingListItems_ShoppingLists_ShoppingListId",
                        column: x => x.ShoppingListId,
                        principalTable: "ShoppingLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoredProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductTemplateId = table.Column<int>(type: "integer", nullable: false),
                    StorageLocationId = table.Column<int>(type: "integer", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProductDetails = table.Column<string>(type: "jsonb", nullable: true),
                    DeleteFl = table.Column<bool>(type: "boolean", nullable: false),
                    RekordChange = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoredProducts_ProductTemplates_ProductTemplateId",
                        column: x => x.ProductTemplateId,
                        principalTable: "ProductTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoredProducts_StorageLocations_StorageLocationId",
                        column: x => x.StorageLocationId,
                        principalTable: "StorageLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StoredProductId = table.Column<int>(type: "integer", nullable: false),
                    MovementType = table.Column<string>(type: "text", nullable: false),
                    FromStorageLocationId = table.Column<int>(type: "integer", nullable: true),
                    ToStorageLocationId = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MovementData = table.Column<string>(type: "jsonb", nullable: true),
                    DeleteFl = table.Column<bool>(type: "boolean", nullable: false),
                    RekordChange = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductMovements_StorageLocations_FromStorageLocationId",
                        column: x => x.FromStorageLocationId,
                        principalTable: "StorageLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductMovements_StorageLocations_ToStorageLocationId",
                        column: x => x.ToStorageLocationId,
                        principalTable: "StorageLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductMovements_StoredProducts_StoredProductId",
                        column: x => x.StoredProductId,
                        principalTable: "StoredProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMovements_FromStorageLocationId",
                table: "ProductMovements",
                column: "FromStorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMovements_StoredProductId",
                table: "ProductMovements",
                column: "StoredProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMovements_ToStorageLocationId",
                table: "ProductMovements",
                column: "ToStorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplates_Brand_Name",
                table: "ProductTemplates",
                columns: new[] { "Brand", "Name" },
                unique: true,
                filter: "\"DeleteFl\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListItems_ProductTemplateId",
                table: "ShoppingListItems",
                column: "ProductTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListItems_ShoppingListId_ProductTemplateId",
                table: "ShoppingListItems",
                columns: new[] { "ShoppingListId", "ProductTemplateId" },
                filter: "\"DeleteFl\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_StoredProducts_ProductTemplateId_StorageLocationId_ExpiryDa~",
                table: "StoredProducts",
                columns: new[] { "ProductTemplateId", "StorageLocationId", "ExpiryDate" },
                filter: "\"DeleteFl\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_StoredProducts_StorageLocationId",
                table: "StoredProducts",
                column: "StorageLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductMovements");

            migrationBuilder.DropTable(
                name: "ShoppingListItems");

            migrationBuilder.DropTable(
                name: "StoredProducts");

            migrationBuilder.DropTable(
                name: "ShoppingLists");

            migrationBuilder.DropTable(
                name: "ProductTemplates");

            migrationBuilder.DropTable(
                name: "StorageLocations");
        }
    }
}
