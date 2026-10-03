using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookTracker.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenamedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorPK",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Genres_GenrePK",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "GenrePK",
                table: "Genres",
                newName: "GenrePk");

            migrationBuilder.RenameColumn(
                name: "GenrePK",
                table: "Books",
                newName: "GenrePk");

            migrationBuilder.RenameColumn(
                name: "AuthorPK",
                table: "Books",
                newName: "AuthorPk");

            migrationBuilder.RenameColumn(
                name: "BookPK",
                table: "Books",
                newName: "BookPk");

            migrationBuilder.RenameIndex(
                name: "IX_Books_GenrePK",
                table: "Books",
                newName: "IX_Books_GenrePk");

            migrationBuilder.RenameIndex(
                name: "IX_Books_AuthorPK",
                table: "Books",
                newName: "IX_Books_AuthorPk");

            migrationBuilder.RenameColumn(
                name: "AuthorPK",
                table: "Authors",
                newName: "AuthorPk");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateRead",
                table: "Books",
                type: "timestamptz",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorPk",
                table: "Books",
                column: "AuthorPk",
                principalTable: "Authors",
                principalColumn: "AuthorPk");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Genres_GenrePk",
                table: "Books",
                column: "GenrePk",
                principalTable: "Genres",
                principalColumn: "GenrePk");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorPk",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Genres_GenrePk",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "GenrePk",
                table: "Genres",
                newName: "GenrePK");

            migrationBuilder.RenameColumn(
                name: "GenrePk",
                table: "Books",
                newName: "GenrePK");

            migrationBuilder.RenameColumn(
                name: "AuthorPk",
                table: "Books",
                newName: "AuthorPK");

            migrationBuilder.RenameColumn(
                name: "BookPk",
                table: "Books",
                newName: "BookPK");

            migrationBuilder.RenameIndex(
                name: "IX_Books_GenrePk",
                table: "Books",
                newName: "IX_Books_GenrePK");

            migrationBuilder.RenameIndex(
                name: "IX_Books_AuthorPk",
                table: "Books",
                newName: "IX_Books_AuthorPK");

            migrationBuilder.RenameColumn(
                name: "AuthorPk",
                table: "Authors",
                newName: "AuthorPK");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateRead",
                table: "Books",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorPK",
                table: "Books",
                column: "AuthorPK",
                principalTable: "Authors",
                principalColumn: "AuthorPK");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Genres_GenrePK",
                table: "Books",
                column: "GenrePK",
                principalTable: "Genres",
                principalColumn: "GenrePK");
        }
    }
}
