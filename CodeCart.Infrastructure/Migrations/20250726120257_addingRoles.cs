using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CodeCart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b7a7bc4f-d8ff-45c2-8d91-3f2bc1c2e123", null, "Admin", "ADMIN" },
                    { "e2a0f3fc-ae47-4b9f-89aa-5870f26bd823", null, "Customer", "CUSTOMER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b7a7bc4f-d8ff-45c2-8d91-3f2bc1c2e123");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e2a0f3fc-ae47-4b9f-89aa-5870f26bd823");
        }
    }
}
