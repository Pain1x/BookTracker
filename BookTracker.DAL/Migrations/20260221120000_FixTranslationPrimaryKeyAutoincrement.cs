using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookTracker.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixTranslationPrimaryKeyAutoincrement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
DO $$
BEGIN
    IF pg_get_serial_sequence('"BookTranslations"', 'BookTranslationPk') IS NULL THEN
        CREATE SEQUENCE IF NOT EXISTS "BookTranslations_BookTranslationPk_seq";
        ALTER TABLE "BookTranslations"
            ALTER COLUMN "BookTranslationPk"
            SET DEFAULT nextval('"BookTranslations_BookTranslationPk_seq"'::regclass);
        ALTER SEQUENCE "BookTranslations_BookTranslationPk_seq"
            OWNED BY "BookTranslations"."BookTranslationPk";
    END IF;

    IF pg_get_serial_sequence('"AuthorTranslations"', 'AuthorTranslationPk') IS NULL THEN
        CREATE SEQUENCE IF NOT EXISTS "AuthorTranslations_AuthorTranslationPk_seq";
        ALTER TABLE "AuthorTranslations"
            ALTER COLUMN "AuthorTranslationPk"
            SET DEFAULT nextval('"AuthorTranslations_AuthorTranslationPk_seq"'::regclass);
        ALTER SEQUENCE "AuthorTranslations_AuthorTranslationPk_seq"
            OWNED BY "AuthorTranslations"."AuthorTranslationPk";
    END IF;

    IF pg_get_serial_sequence('"GenreTranslations"', 'GenreTranslationPk') IS NULL THEN
        CREATE SEQUENCE IF NOT EXISTS "GenreTranslations_GenreTranslationPk_seq";
        ALTER TABLE "GenreTranslations"
            ALTER COLUMN "GenreTranslationPk"
            SET DEFAULT nextval('"GenreTranslations_GenreTranslationPk_seq"'::regclass);
        ALTER SEQUENCE "GenreTranslations_GenreTranslationPk_seq"
            OWNED BY "GenreTranslations"."GenreTranslationPk";
    END IF;
END $$;
""");

            migrationBuilder.Sql(
                """
SELECT setval(
    pg_get_serial_sequence('"BookTranslations"', 'BookTranslationPk'),
    COALESCE((SELECT MAX("BookTranslationPk") + 1 FROM "BookTranslations"), 1),
    false);
""");

            migrationBuilder.Sql(
                """
SELECT setval(
    pg_get_serial_sequence('"AuthorTranslations"', 'AuthorTranslationPk'),
    COALESCE((SELECT MAX("AuthorTranslationPk") + 1 FROM "AuthorTranslations"), 1),
    false);
""");

            migrationBuilder.Sql(
                """
SELECT setval(
    pg_get_serial_sequence('"GenreTranslations"', 'GenreTranslationPk'),
    COALESCE((SELECT MAX("GenreTranslationPk") + 1 FROM "GenreTranslations"), 1),
    false);
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
