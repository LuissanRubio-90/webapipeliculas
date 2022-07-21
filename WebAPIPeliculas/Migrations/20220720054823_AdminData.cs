using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIPeliculas.Migrations
{
    public partial class AdminData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f6533ab4-ddf0-4e9a-b74a-e80e78887f2e", "1c1d1bb6-6805-4028-82c6-12e8da5db5c6", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "aa2ce5ce-7744-47c1-bdf4-8a2bd01f5ddc", 0, "196e7bb4-bcd0-4517-b298-310d5820e961", "luisrubio_dev90@outlook.com", false, false, null, "luisrubio_dev90@outlook.com", "luisrubio_dev90@outlook.com", "AQAAAAEAACcQAAAAEMHUslp6wczWRUMIwUTRFUiO4Fw+JQAi8TSbRpho6dru0sDxiiRyN6p7EQmTNug/sw==", null, false, "d39d43fb-14d7-4e02-80de-3a1af2f0480e", false, "luisrubio_dev90@outlook.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[] { 1, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin", "aa2ce5ce-7744-47c1-bdf4-8a2bd01f5ddc" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f6533ab4-ddf0-4e9a-b74a-e80e78887f2e");

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "aa2ce5ce-7744-47c1-bdf4-8a2bd01f5ddc");
        }
    }
}
