using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowtap_Food.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class secondmigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecipientContact",
                table: "StockAlertRules",
                newName: "WhatsAppRecipients");

            migrationBuilder.AddColumn<string>(
                name: "EmailRecipients",
                table: "StockAlertRules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmsRecipients",
                table: "StockAlertRules",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailRecipients",
                table: "StockAlertRules");

            migrationBuilder.DropColumn(
                name: "SmsRecipients",
                table: "StockAlertRules");

            migrationBuilder.RenameColumn(
                name: "WhatsAppRecipients",
                table: "StockAlertRules",
                newName: "RecipientContact");
        }
    }
}
