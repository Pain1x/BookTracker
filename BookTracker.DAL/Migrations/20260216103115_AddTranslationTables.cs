using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookTracker.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguagePk = table.Column<byte>(type: "smallint", nullable: false),
                    LanguageName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguagePk);
                });

            migrationBuilder.CreateTable(
                name: "AuthorTranslations",
                columns: table => new
                {
                    AuthorTranslationPk = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorPk = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguagePk = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorTranslations", x => x.AuthorTranslationPk);
                    table.ForeignKey(
                        name: "FK_AuthorTranslations_Authors_AuthorPk",
                        column: x => x.AuthorPk,
                        principalTable: "Authors",
                        principalColumn: "AuthorPk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorTranslations_Languages_LanguagePk",
                        column: x => x.LanguagePk,
                        principalTable: "Languages",
                        principalColumn: "LanguagePk");
                });

            migrationBuilder.CreateTable(
                name: "BookTranslations",
                columns: table => new
                {
                    BookTranslationPk = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookPk = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguagePk = table.Column<byte>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookTranslations", x => x.BookTranslationPk);
                    table.ForeignKey(
                        name: "FK_BookTranslations_Books_BookPk",
                        column: x => x.BookPk,
                        principalTable: "Books",
                        principalColumn: "BookPk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookTranslations_Languages_LanguagePk",
                        column: x => x.LanguagePk,
                        principalTable: "Languages",
                        principalColumn: "LanguagePk");
                });

            migrationBuilder.CreateTable(
                name: "GenreTranslations",
                columns: table => new
                {
                    GenreTranslationPk = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GenrePk = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguagePk = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreTranslations", x => x.GenreTranslationPk);
                    table.ForeignKey(
                        name: "FK_GenreTranslations_Genres_GenrePk",
                        column: x => x.GenrePk,
                        principalTable: "Genres",
                        principalColumn: "GenrePk",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreTranslations_Languages_LanguagePk",
                        column: x => x.LanguagePk,
                        principalTable: "Languages",
                        principalColumn: "LanguagePk");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorTranslations_AuthorPk_LanguagePk",
                table: "AuthorTranslations",
                columns: new[] { "AuthorPk", "LanguagePk" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorTranslations_LanguagePk",
                table: "AuthorTranslations",
                column: "LanguagePk");

            migrationBuilder.CreateIndex(
                name: "IX_BookTranslations_BookPk_LanguagePk",
                table: "BookTranslations",
                columns: new[] { "BookPk", "LanguagePk" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookTranslations_LanguagePk",
                table: "BookTranslations",
                column: "LanguagePk");

            migrationBuilder.CreateIndex(
                name: "IX_GenreTranslations_GenrePk_LanguagePk",
                table: "GenreTranslations",
                columns: new[] { "GenrePk", "LanguagePk" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenreTranslations_LanguagePk",
                table: "GenreTranslations",
                column: "LanguagePk");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorTranslations");

            migrationBuilder.DropTable(
                name: "BookTranslations");

            migrationBuilder.DropTable(
                name: "GenreTranslations");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
