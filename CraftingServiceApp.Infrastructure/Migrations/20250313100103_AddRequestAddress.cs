using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingServiceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_AspNetUsers_ClientId",
                table: "Address");

            migrationBuilder.AddColumn<string>(
                name: "CustomCity",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomCountry",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomPostalCode",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomStreet",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelectedAddressId",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SelectedAddressId",
                table: "Requests",
                column: "SelectedAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AspNetUsers_ClientId",
                table: "Address",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Address_SelectedAddressId",
                table: "Requests",
                column: "SelectedAddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_AspNetUsers_ClientId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Address_SelectedAddressId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_SelectedAddressId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CustomCity",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CustomCountry",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CustomPostalCode",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CustomStreet",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "SelectedAddressId",
                table: "Requests");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AspNetUsers_ClientId",
                table: "Address",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
