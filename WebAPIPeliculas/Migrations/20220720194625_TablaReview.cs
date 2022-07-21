using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIPeliculas.Migrations
{
    public partial class TablaReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f6533ab4-ddf0-4e9a-b74a-e80e78887f2e",
                column: "ConcurrencyStamp",
                value: "f68de618-fc4d-45f8-944f-21c2d8bee6ee");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "aa2ce5ce-7744-47c1-bdf4-8a2bd01f5ddc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4de8da81-69fd-44d6-8e4d-a2617f26e84a", "AQAAAAEAACcQAAAAELHxWA6YnyS2uHZjMey2O5CpUaUXZH1djGHzG3KDc1ROIQh2DM0X2THBiAX0zVWe+g==", "35af25b3-ed47-4b78-92ed-dd62290c22c2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f6533ab4-ddf0-4e9a-b74a-e80e78887f2e",
                column: "ConcurrencyStamp",
                value: "901b7a4a-a35f-440b-b24b-c49b3719aaa1");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "aa2ce5ce-7744-47c1-bdf4-8a2bd01f5ddc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b228184e-1387-4291-b70f-c66b6c330163", "AQAAAAEAACcQAAAAEBKD+em0hRoQ/CxgijSHepcYsGmygX57oSyhtj3anvpDCs2i8QS27E1wbLi/Rw1e0Q==", "a522631f-0e98-4758-a2a1-0cb59a3c3796" });
        }
    }
}
