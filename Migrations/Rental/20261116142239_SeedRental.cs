using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASPCoreFirstApp.Migrations.Rental
{
    /// <inheritdoc />
    public partial class SeedRental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [rental].[MembershipTypes] WHERE [Id] = 1)
                    INSERT INTO [rental].[MembershipTypes] ([Id], [Name], [DiscountPercent])
                    VALUES (1, 'Standard', 0);
                IF NOT EXISTS (SELECT 1 FROM [rental].[MembershipTypes] WHERE [Id] = 2)
                    INSERT INTO [rental].[MembershipTypes] ([Id], [Name], [DiscountPercent])
                    VALUES (2, 'Premium', 10);
                IF NOT EXISTS (SELECT 1 FROM [rental].[MembershipTypes] WHERE [Id] = 3)
                    INSERT INTO [rental].[MembershipTypes] ([Id], [Name], [DiscountPercent])
                    VALUES (3, 'Gold', 20);

                IF NOT EXISTS (SELECT 1 FROM [rental].[Customers] WHERE [Id] = 1)
                    INSERT INTO [rental].[Customers] ([Id], [Name], [Email], [MembershipTypeId])
                    VALUES (1, 'Amal Ben Salah', 'amal@example.com', 2);
                IF NOT EXISTS (SELECT 1 FROM [rental].[Customers] WHERE [Id] = 2)
                    INSERT INTO [rental].[Customers] ([Id], [Name], [Email], [MembershipTypeId])
                    VALUES (2, 'Youssef Trabelsi', 'youssef@example.com', 1);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "rental",
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "rental",
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "rental",
                table: "MembershipTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "rental",
                table: "MembershipTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "rental",
                table: "MembershipTypes",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
