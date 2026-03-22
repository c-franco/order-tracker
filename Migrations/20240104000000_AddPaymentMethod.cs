using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderTracker.Migrations
{
    public partial class AddPaymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_PaymentMethods", x => x.Id); });

            migrationBuilder.InsertData("PaymentMethods", new[] { "Id", "Name", "SortOrder" }, new object[,] {
                { 1, "Tarjeta de crédito", 1 },
                { 2, "Tarjeta de débito", 2 },
                { 3, "PayPal", 3 },
                { 4, "Bizum", 4 },
                { 5, "Transferencia", 5 },
                { 6, "Contra reembolso", 6 }
            });

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex("IX_Orders_PaymentMethodId", "Orders", "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentMethods_PaymentMethodId",
                table: "Orders",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_Orders_PaymentMethods_PaymentMethodId", "Orders");
            migrationBuilder.DropIndex("IX_Orders_PaymentMethodId", "Orders");
            migrationBuilder.DropColumn("PaymentMethodId", "Orders");
            migrationBuilder.DropTable("PaymentMethods");
        }
    }
}
