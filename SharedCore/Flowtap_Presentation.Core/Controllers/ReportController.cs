using Flowtap_Application.Features.Reports.Queries.GetDashboardStats;
using Flowtap_Application.Features.Reports.Queries.GetInventoryReport;
using Flowtap_Application.Features.Reports.Queries.GetSalesReport;
using Flowtap_Application.Features.Reports.Queries.GetTopProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Flowtap_Presentation.Controllers;

[Flowtap_Presentation.Authorization.RequirePermission("Reports")]
[Route("api/v1/reports")]
public class ReportController(ISender sender) : ApiController(sender)
{
    [HttpGet("sales")]
    public async Task<IActionResult> GetSalesReport([FromQuery] GetSalesReportQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("sales/export")]
    public async Task<IActionResult> ExportSales([FromQuery] GetSalesReportQuery query, CancellationToken ct)
    {
        var result = await Sender.Send(query, ct);
        if (!result.IsSuccess) return BadRequest(result.Error);

        var report = result.Value!;
        var sb = new StringBuilder();
        sb.AppendLine("Date,Revenue,Transactions");
        foreach (var day in report.DailySummary)
        {
            sb.AppendLine($"{day.Date:yyyy-MM-dd},{day.Revenue},{day.Transactions}");
        }
        sb.AppendLine($"TOTAL,{report.TotalRevenue},{report.TotalTransactions}");

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", $"sales-report-{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard([FromQuery] GetDashboardStatsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("inventory")]
    public async Task<IActionResult> InventoryReport([FromQuery] GetInventoryReportQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));

    [HttpGet("top-products")]
    public async Task<IActionResult> TopProducts([FromQuery] GetTopProductsQuery query, CancellationToken ct)
        => Ok(await Sender.Send(query, ct));
}
