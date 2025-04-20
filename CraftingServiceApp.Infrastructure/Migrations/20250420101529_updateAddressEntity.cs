using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingServiceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateAddressEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Address",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Address");
        }
    }
}
