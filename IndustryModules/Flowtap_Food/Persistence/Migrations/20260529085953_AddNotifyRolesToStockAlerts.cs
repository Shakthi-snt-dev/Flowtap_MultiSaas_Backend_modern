using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowtap_Food.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifyRolesToStockAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotifyRoles",
                table: "StockAlertRules",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyRoles",
                table: "StockAlertRules");
        }
    }
}
