using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeWise.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "persons",
                columns: new[] { "id", "name", "email" },
                values: new object[,]
                {
                    { Guid.Parse("11111111-1111-1111-1111-111111111111"), "Bob", "bob@bertram.com" },
                    { Guid.Parse("22222222-2222-2222-2222-222222222222"), "Jeremy", "jeremy@bertram.com" },
                    { Guid.Parse("33333333-3333-3333-3333-333333333333"), "Alice", "alice@bertram.com" },
                    { Guid.Parse("44444444-4444-4444-4444-444444444444"), "Grace", "grace@bertram.com" },
                    { Guid.Parse("55555555-5555-5555-5555-555555555555"), "Eve", "eve@bertram.com" },
                    { Guid.Parse("66666666-6666-6666-6666-666666666666"), "Mallory", "mallory@bertram.com" },
                    { Guid.Parse("77777777-7777-7777-7777-777777777777"), "Trent", "trent@bertram.com" }
                }
            );

            migrationBuilder.InsertData(
                table: "groups",
                columns: new[] { "id", "name" },
                values: new object[] 
                { 
                    Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                    "Bertram Labs Office" 
                }
            );

            migrationBuilder.InsertData(
                table: "group_members",
                columns: new[] { "id", "group_id", "person_id" },
                values: new object[,]
                {
                    { Guid.Parse("bbbbbbbb-0000-0000-0000-000000000001"), Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Guid.Parse("11111111-1111-1111-1111-111111111111") },
                    { Guid.Parse("bbbbbbbb-0000-0000-0000-000000000002"), Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Guid.Parse("22222222-2222-2222-2222-222222222222") },
                    { Guid.Parse("bbbbbbbb-0000-0000-0000-000000000003"), Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Guid.Parse("33333333-3333-3333-3333-333333333333") },
                    { Guid.Parse("bbbbbbbb-0000-0000-0000-000000000004"), Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Guid.Parse("44444444-4444-4444-4444-444444444444") },
                    { Guid.Parse("bbbbbbbb-0000-0000-0000-000000000005"), Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Guid.Parse("55555555-5555-5555-5555-555555555555") },
                    { Guid.Parse("bbbbbbbb-0000-0000-0000-000000000006"), Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Guid.Parse("66666666-6666-6666-6666-666666666666") },
                    { Guid.Parse("bbbbbbbb-0000-0000-0000-000000000007"), Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Guid.Parse("77777777-7777-7777-7777-777777777777") }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
