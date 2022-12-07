using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pantry.Core.Models.EanSearchOrg;
using Pantry.Core.Models.OpenFoodFacts;

#nullable disable

namespace Pantry.Core.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "metadatas",
                columns: table => new
                {
                    metadataid = table.Column<long>(name: "metadata_id", type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    globaltradeitemnumber = table.Column<string>(name: "global_trade_item_number", type: "text", nullable: false),
                    foodfacts = table.Column<Product>(name: "food_facts", type: "jsonb", nullable: true),
                    productfacts = table.Column<NonFoodProduct>(name: "product_facts", type: "jsonb", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_metadatas", x => x.metadataid);
                });

            migrationBuilder.CreateIndex(
                name: "ix_metadatas_global_trade_item_number",
                table: "metadatas",
                column: "global_trade_item_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "metadatas");
        }
    }
}
