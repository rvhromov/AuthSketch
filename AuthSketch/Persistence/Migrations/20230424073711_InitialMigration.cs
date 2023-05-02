using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthSketch.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    Salt = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<byte>(type: "smallint", nullable: false),
                    VerificationToken = table.Column<string>(type: "text", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResetToken = table.Column<string>(type: "text", nullable: true),
                    ResetTokenExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResetTokenUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsTfaEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TfaKey = table.Column<string>(type: "text", nullable: true),
                    TfaEnabledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalAuthProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Provider = table.Column<byte>(type: "smallint", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalAuthProviders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalAuthProviders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokedByIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReplacedByToken = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsTfaEnabled", "ModifiedAt", "Name", "PasswordHash", "ResetToken", "ResetTokenExpiresAt", "ResetTokenUsedAt", "Role", "Salt", "TfaEnabledAt", "TfaKey", "VerificationToken", "VerifiedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 4, 24, 7, 37, 11, 381, DateTimeKind.Utc).AddTicks(3437), "admin@auth.com", false, null, "Admin", "8AA0FCD19E374352DA62AEA1F176E8ACBBE3C4B9ECF7B5520EF514E3C0CDAE5C9195D6DC39580919EA569FD4B14C5A5F2DB0A882BE221E4A597BC6F8CE5AEA5D", null, null, null, (byte)1, "6A5EF455C2FF6F3C554831666058A1943C0252ED0FC47BF4C1B3E39462F575A45CE8D3688C65E566FCBDABE3309DA91511051668E4B0EC6278E7D10BFF3A2B95", null, null, "DB96319BCB57D86D22C1AF2478F65AF198681171E97040E6552D2603C6C413E0CE7F62D7CCC8F90A106AF5C4F1867A35C7AC291E452D823EC797BDCC7F7146D9", new DateTime(2023, 4, 24, 7, 37, 11, 381, DateTimeKind.Utc).AddTicks(3440) },
                    { 2, new DateTime(2023, 4, 24, 7, 37, 11, 381, DateTimeKind.Utc).AddTicks(3442), "user@auth.com", false, null, "User", "8AA0FCD19E374352DA62AEA1F176E8ACBBE3C4B9ECF7B5520EF514E3C0CDAE5C9195D6DC39580919EA569FD4B14C5A5F2DB0A882BE221E4A597BC6F8CE5AEA5D", null, null, null, (byte)0, "6A5EF455C2FF6F3C554831666058A1943C0252ED0FC47BF4C1B3E39462F575A45CE8D3688C65E566FCBDABE3309DA91511051668E4B0EC6278E7D10BFF3A2B95", null, null, "DB96319BCB57D86D22C1AF2478F65AF198681171E97040E6552D2603C6C413E0CE7F62D7CCC8F90A106AF5C4F1867A35C7AC291E452D823EC797BDCC7F7146D9", new DateTime(2023, 4, 24, 7, 37, 11, 381, DateTimeKind.Utc).AddTicks(3442) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAuthProviders_UserId",
                table: "ExternalAuthProviders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalAuthProviders");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
