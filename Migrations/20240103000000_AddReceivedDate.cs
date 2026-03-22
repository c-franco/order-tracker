using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderTracker.Migrations
{
    public partial class AddReceivedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDate",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ReceivedDate", table: "Orders");
        }
    }
}
