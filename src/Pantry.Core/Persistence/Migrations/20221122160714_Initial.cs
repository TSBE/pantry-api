using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pantry.Core.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "households",
                columns: table => new
                {
                    householdid = table.Column<long>(name: "household_id", type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    subscriptiontype = table.Column<int>(name: "subscription_type", type: "integer", nullable: false),
                    ownerid = table.Column<long>(name: "owner_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_households", x => x.householdid);
                });

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    firstname = table.Column<string>(name: "first_name", type: "text", nullable: false),
                    lastname = table.Column<string>(name: "last_name", type: "text", nullable: false),
                    friendscode = table.Column<Guid>(name: "friends_code", type: "uuid", nullable: false),
                    oauhtid = table.Column<string>(name: "o_auht_id", type: "text", nullable: false),
                    householdid = table.Column<long>(name: "household_id", type: "bigint", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts", x => x.accountid);
                    table.ForeignKey(
                        name: "fk_accounts_households_household_id",
                        column: x => x.householdid,
                        principalTable: "households",
                        principalColumn: "household_id");
                });

            migrationBuilder.CreateTable(
                name: "storage_locations",
                columns: table => new
                {
                    storagelocationid = table.Column<long>(name: "storage_location_id", type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    householdid = table.Column<long>(name: "household_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_storage_locations", x => x.storagelocationid);
                    table.ForeignKey(
                        name: "fk_storage_locations_households_household_id",
                        column: x => x.householdid,
                        principalTable: "households",
                        principalColumn: "household_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    deviceid = table.Column<long>(name: "device_id", type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    model = table.Column<string>(type: "text", nullable: false),
                    devicetoken = table.Column<string>(name: "device_token", type: "text", nullable: true),
                    platform = table.Column<int>(type: "integer", nullable: false),
                    installationid = table.Column<Guid>(name: "installation_id", type: "uuid", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices", x => x.deviceid);
                    table.ForeignKey(
                        name: "fk_devices_accounts_account_id",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invitations",
                columns: table => new
                {
                    invitationid = table.Column<long>(name: "invitation_id", type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    validuntildate = table.Column<DateTime>(name: "valid_until_date", type: "timestamp with time zone", nullable: false),
                    friendscode = table.Column<Guid>(name: "friends_code", type: "uuid", nullable: false),
                    creatorid = table.Column<long>(name: "creator_id", type: "bigint", nullable: false),
                    householdid = table.Column<long>(name: "household_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitations", x => x.invitationid);
                    table.ForeignKey(
                        name: "fk_invitations_accounts_creator_id",
                        column: x => x.creatorid,
                        principalTable: "accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_invitations_households_household_id",
                        column: x => x.householdid,
                        principalTable: "households",
                        principalColumn: "household_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    articleid = table.Column<long>(name: "article_id", type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    globaltradeitemnumber = table.Column<string>(name: "global_trade_item_number", type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    bestbeforedate = table.Column<DateTime>(name: "best_before_date", type: "timestamp with time zone", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    contenttype = table.Column<int>(name: "content_type", type: "integer", nullable: false),
                    storagelocationid = table.Column<long>(name: "storage_location_id", type: "bigint", nullable: false),
                    householdid = table.Column<long>(name: "household_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_articles", x => x.articleid);
                    table.ForeignKey(
                        name: "fk_articles_households_household_id",
                        column: x => x.householdid,
                        principalTable: "households",
                        principalColumn: "household_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_articles_storage_locations_storage_location_id",
                        column: x => x.storagelocationid,
                        principalTable: "storage_locations",
                        principalColumn: "storage_location_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_household_id",
                table: "accounts",
                column: "household_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_o_auht_id",
                table: "accounts",
                column: "o_auht_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_articles_best_before_date",
                table: "articles",
                column: "best_before_date");

            migrationBuilder.CreateIndex(
                name: "ix_articles_global_trade_item_number",
                table: "articles",
                column: "global_trade_item_number");

            migrationBuilder.CreateIndex(
                name: "ix_articles_household_id",
                table: "articles",
                column: "household_id");

            migrationBuilder.CreateIndex(
                name: "ix_articles_storage_location_id",
                table: "articles",
                column: "storage_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_devices_account_id",
                table: "devices",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_devices_device_token",
                table: "devices",
                column: "device_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_devices_installation_id",
                table: "devices",
                column: "installation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invitations_creator_id",
                table: "invitations",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_invitations_friends_code",
                table: "invitations",
                column: "friends_code");

            migrationBuilder.CreateIndex(
                name: "ix_invitations_household_id",
                table: "invitations",
                column: "household_id");

            migrationBuilder.CreateIndex(
                name: "ix_storage_locations_household_id",
                table: "storage_locations",
                column: "household_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articles");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "invitations");

            migrationBuilder.DropTable(
                name: "storage_locations");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "households");
        }
    }
}
