using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASPCoreFirstApp.Migrations.Catalog
{
    /// <inheritdoc />
    public partial class SeedCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE [Movies] DROP CONSTRAINT [FK_Movies_Genres_GenreId];
                DROP INDEX [IX_Movies_GenreId] ON [Movies];

                ALTER TABLE [Genres] ADD [NewId] uniqueidentifier NULL;
                """);

            migrationBuilder.Sql("""
                UPDATE [Genres]
                SET [NewId] = CASE [Id]
                    WHEN 1 THEN '84ca0bcd-082c-49cb-aa77-ea2f1f5f8285'
                    WHEN 2 THEN '79e6f638-d7e7-4f63-8365-f172cb925381'
                    WHEN 3 THEN '3f2b8c4e-5a1d-4e7b-9c60-1d2e3f4a5b6c'
                    ELSE NEWID()
                END;

                ALTER TABLE [Movies] ADD [NewGenreId] uniqueidentifier NULL;
                """);

            migrationBuilder.Sql("""
                UPDATE movies
                SET [NewGenreId] = genres.[NewId]
                FROM [Movies] movies
                INNER JOIN [Genres] genres ON genres.[Id] = movies.[GenreId];
                """);

            migrationBuilder.Sql("""
                DECLARE @defaultConstraint nvarchar(128);
                SELECT @defaultConstraint = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c]
                    ON [c].[default_object_id] = [d].[object_id]
                WHERE [d].[parent_object_id] = OBJECT_ID(N'[Movies]')
                  AND [c].[name] = N'GenreId';
                IF @defaultConstraint IS NOT NULL
                    EXEC(N'ALTER TABLE [Movies] DROP CONSTRAINT [' + @defaultConstraint + N']');
                """);

            migrationBuilder.Sql("""
                ALTER TABLE [Movies] DROP COLUMN [GenreId];
                """);

            migrationBuilder.Sql("""
                ALTER TABLE [Genres] DROP CONSTRAINT [PK_Genres];
                ALTER TABLE [Genres] DROP COLUMN [Id];
                EXEC sp_rename 'Genres.NewId', 'Id', 'COLUMN';
                EXEC sp_rename 'Movies.NewGenreId', 'GenreId', 'COLUMN';
                """);

            migrationBuilder.Sql("""
                ALTER TABLE [Genres] ALTER COLUMN [Id] uniqueidentifier NOT NULL;
                ALTER TABLE [Movies] ALTER COLUMN [GenreId] uniqueidentifier NOT NULL;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE [Genres] ADD CONSTRAINT [PK_Genres] PRIMARY KEY ([Id]);
                CREATE INDEX [IX_Movies_GenreId] ON [Movies] ([GenreId]);
                ALTER TABLE [Movies] ADD CONSTRAINT [FK_Movies_Genres_GenreId]
                    FOREIGN KEY ([GenreId]) REFERENCES [Genres] ([Id]) ON DELETE CASCADE;

                UPDATE [Genres]
                SET [Name] = CASE CONVERT(varchar(36), [Id])
                    WHEN '84ca0bcd-082c-49cb-aa77-ea2f1f5f8285' THEN 'Action'
                    WHEN '79e6f638-d7e7-4f63-8365-f172cb925381' THEN 'Drame'
                    WHEN '3f2b8c4e-5a1d-4e7b-9c60-1d2e3f4a5b6c' THEN 'Science-fiction'
                    ELSE [Name]
                END;
                """);

            migrationBuilder.Sql("""
                SET IDENTITY_INSERT [Movies] ON;
                IF NOT EXISTS (SELECT 1 FROM [Movies] WHERE [Id] = 1)
                    INSERT INTO [Movies] ([Id], [Name], [GenreId], [IsAvailable])
                    VALUES (1, 'Inception', '3f2b8c4e-5a1d-4e7b-9c60-1d2e3f4a5b6c', 1);
                IF NOT EXISTS (SELECT 1 FROM [Movies] WHERE [Id] = 2)
                    INSERT INTO [Movies] ([Id], [Name], [GenreId], [IsAvailable])
                    VALUES (2, 'Gladiator', '84ca0bcd-082c-49cb-aa77-ea2f1f5f8285', 1);
                IF NOT EXISTS (SELECT 1 FROM [Movies] WHERE [Id] = 3)
                    INSERT INTO [Movies] ([Id], [Name], [GenreId], [IsAvailable])
                    VALUES (3, 'Interstellar', '3f2b8c4e-5a1d-4e7b-9c60-1d2e3f4a5b6c', 1);
                SET IDENTITY_INSERT [Movies] OFF;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("THROW 50000, 'SeedCatalog cannot be safely reverted because existing GUID mappings may be in use.', 1;");
        }
    }
}
