using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftingServiceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RequestWorkFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_AspNetUsers_ClientId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AspNetUsers_ClientId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AspNetUsers_CrafterId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Services_ServiceId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_ClientId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Payments_PaymentId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_AspNetUsers_CrafterId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Categories_CategoryId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_userPayments_Services_ServiceId",
                table: "userPayments");

            migrationBuilder.DropIndex(
                name: "IX_userPayments_ServiceId",
                table: "userPayments");

            migrationBuilder.DropIndex(
                name: "IX_Requests_PaymentId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ClientId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CrafterId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ServiceId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Payments",
                newName: "RequestId");

            migrationBuilder.RenameColumn(
                name: "PaymentDate",
                table: "Payments",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "userPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDateTime",
                table: "Requests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelectedScheduleId",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CrafterId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "requestSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    ProposedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requestSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requestSchedules_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userPayments_RequestId",
                table: "userPayments",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_PaymentId",
                table: "Requests",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SelectedScheduleId",
                table: "Requests",
                column: "SelectedScheduleId",
                unique: true,
                filter: "[SelectedScheduleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_requestSchedules_RequestId",
                table: "requestSchedules",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AspNetUsers_ClientId",
                table: "Address",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_ClientId",
                table: "Posts",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Payments_PaymentId",
                table: "Requests",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_requestSchedules_SelectedScheduleId",
                table: "Requests",
                column: "SelectedScheduleId",
                principalTable: "requestSchedules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_AspNetUsers_CrafterId",
                table: "Services",
                column: "CrafterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Categories_CategoryId",
                table: "Services",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_userPayments_Requests_RequestId",
                table: "userPayments",
                column: "RequestId",
                principalTable: "Requests",
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
                name: "FK_Posts_AspNetUsers_ClientId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Payments_PaymentId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_requestSchedules_SelectedScheduleId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_AspNetUsers_CrafterId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Categories_CategoryId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_userPayments_Requests_RequestId",
                table: "userPayments");

            migrationBuilder.DropTable(
                name: "requestSchedules");

            migrationBuilder.DropIndex(
                name: "IX_userPayments_RequestId",
                table: "userPayments");

            migrationBuilder.DropIndex(
                name: "IX_Requests_PaymentId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_SelectedScheduleId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "userPayments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ScheduledDateTime",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "SelectedScheduleId",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "Payments",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Payments",
                newName: "PaymentDate");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Requests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CrafterId",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_userPayments_ServiceId",
                table: "userPayments",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_PaymentId",
                table: "Requests",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ClientId",
                table: "Payments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CrafterId",
                table: "Payments",
                column: "CrafterId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ServiceId",
                table: "Payments",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AspNetUsers_ClientId",
                table: "Address",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AspNetUsers_ClientId",
                table: "Payments",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AspNetUsers_CrafterId",
                table: "Payments",
                column: "CrafterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Services_ServiceId",
                table: "Payments",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_ClientId",
                table: "Posts",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Payments_PaymentId",
                table: "Requests",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_AspNetUsers_CrafterId",
                table: "Services",
                column: "CrafterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Categories_CategoryId",
                table: "Services",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userPayments_Services_ServiceId",
                table: "userPayments",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
