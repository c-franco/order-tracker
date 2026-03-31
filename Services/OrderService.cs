using Microsoft.EntityFrameworkCore;
using OrderTracker.Data;
using OrderTracker.DTOs;
using OrderTracker.Models;
using OrderTracker.Resources;

namespace OrderTracker.Services;

public class OrderService
{
    private readonly AppDbContext _db;
    private readonly CarrierService _carrierService;

    public OrderService(AppDbContext db, CarrierService carrierService)
    {
        _db = db;
        _carrierService = carrierService;
    }

    public async Task<List<Order>> GetAllAsync(string? search = null, OrderStatus? statusFilter = null)
    {
        var query = _db.Orders
            .Include(o => o.Items)
            .Include(o => o.Carrier)
            .Include(o => o.PaymentMethod)
            .Where(o => !o.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(o =>
                o.Store.ToLower().Contains(s) ||
                (o.TrackingCode != null && o.TrackingCode.ToLower().Contains(s)));
        }

        if (statusFilter.HasValue)
            query = query.Where(o => o.Status == statusFilter.Value);

        return await query.OrderByDescending(o => o.PurchaseDate).ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.Carrier)
            .Include(o => o.PaymentMethod)
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        if (!string.IsNullOrWhiteSpace(order.TrackingCode) && string.IsNullOrWhiteSpace(order.TrackingUrl) && order.CarrierId.HasValue)
        {
            var carrier = await _carrierService.GetByIdAsync(order.CarrierId.Value);
            order.TrackingUrl = _carrierService.GetTrackingUrl(carrier, order.TrackingCode);
        }

        RecalculateTotal(order);
        order.CreatedAt = DateTime.Now;
        order.UpdatedAt = DateTime.Now;

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateAsync(Order order)
    {
        var existing = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == order.Id && !o.IsDeleted);
        if (existing == null) return null;

        existing.Store = order.Store;
        existing.PurchaseDate = order.PurchaseDate;
        existing.CarrierId = order.CarrierId;
        existing.TrackingCode = order.TrackingCode;
        existing.Status = order.Status;
        existing.EstimatedDelivery = order.EstimatedDelivery;
        existing.ReceivedDate = order.ReceivedDate;
        existing.PaymentMethodId = order.PaymentMethodId;
        existing.Notes = order.Notes;
        if (order.Status == OrderStatus.Recibido && existing.ReceivedDate == null)
            existing.ReceivedDate = DateTime.Today;
        existing.UpdatedAt = DateTime.Now;

        if (!string.IsNullOrWhiteSpace(order.TrackingCode) && string.IsNullOrWhiteSpace(order.TrackingUrl) && order.CarrierId.HasValue)
        {
            var carrier = await _carrierService.GetByIdAsync(order.CarrierId.Value);
            existing.TrackingUrl = _carrierService.GetTrackingUrl(carrier, order.TrackingCode);
        }
        else
        {
            existing.TrackingUrl = string.IsNullOrWhiteSpace(order.TrackingCode) ? null : order.TrackingUrl;
        }

        _db.OrderItems.RemoveRange(existing.Items);
        existing.Items = order.Items;

        RecalculateTotal(existing);
        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> UpdateStatusAsync(int id, OrderStatus status)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null || order.IsDeleted) return false;

        order.Status = status;
        order.UpdatedAt = DateTime.Now;
        if (status == OrderStatus.Recibido && order.ReceivedDate == null)
            order.ReceivedDate = DateTime.Today;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return false;

        order.IsDeleted = true;
        order.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<DashboardMetrics> GetMetricsAsync()
    {
        var orders = await _db.Orders.Where(o => !o.IsDeleted).ToListAsync();

        return new DashboardMetrics
        {
            TotalOrders = orders.Count,
            TotalPending = orders.Count(o => o.Status != OrderStatus.Recibido && o.Status != OrderStatus.Cancelado),
            InDelivery = orders.Count(o => o.Status == OrderStatus.EnReparto),
            Delayed = orders.Count(o =>
                o.EstimatedDelivery.HasValue &&
                o.EstimatedDelivery.Value.Date < DateTime.Today &&
                o.Status != OrderStatus.Recibido &&
                o.Status != OrderStatus.Cancelado),
            TotalSpent = orders
                .Where(o => o.Status != OrderStatus.Cancelado)
                .Sum(o => o.TotalPrice)
        };
    }

    public async Task<List<CarrierDeliveryStats>> GetCarrierDeliveryStatsAsync()
    {
        var orders = await _db.Orders
            .Include(o => o.Carrier)
            .Where(o => !o.IsDeleted &&
                        o.Status == OrderStatus.Recibido &&
                        o.ReceivedDate.HasValue &&
                        o.CarrierId.HasValue)
            .ToListAsync();

        return orders
            .GroupBy(o => o.Carrier?.Name ?? AppText.CsvUnknownCarrier)
            .Select(g => new CarrierDeliveryStats
            {
                CarrierName = g.Key,
                AvgDays = g.Average(o => (o.ReceivedDate!.Value.Date - o.PurchaseDate.Date).TotalDays),
                OrderCount = g.Count()
            })
            .OrderBy(s => s.AvgDays)
            .ToList();
    }

    public async Task<string> ExportToCsvAsync()
    {
        var orders = await _db.Orders
            .Include(o => o.Carrier)
            .Include(o => o.PaymentMethod)
            .Include(o => o.Items)
            .Where(o => !o.IsDeleted)
            .OrderByDescending(o => o.PurchaseDate)
            .ToListAsync();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine(AppText.CsvHeaderLine);

        foreach (var order in orders)
        {
            var products = string.Join(" | ", order.Items.Select(i => AppText.FormatCsvProduct(i.ProductName, i.Quantity, i.UnitPrice)));

            sb.AppendLine(string.Join(",",
                Escape(order.Store),
                order.PurchaseDate.ToString("dd/MM/yyyy"),
                Escape(order.Carrier?.Name ?? string.Empty),
                Escape(order.TrackingCode ?? string.Empty),
                Escape(order.TrackingUrl ?? string.Empty),
                Escape(AppText.GetOrderStatusLabel(order.Status)),
                order.EstimatedDelivery?.ToString("dd/MM/yyyy") ?? string.Empty,
                order.ReceivedDate?.ToString("dd/MM/yyyy") ?? string.Empty,
                order.TotalPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                Escape(order.PaymentMethod?.Name ?? string.Empty),
                Escape(order.Notes ?? string.Empty),
                Escape(products)
            ));
        }

        return sb.ToString();
    }

    public async Task<(int imported, int skipped, List<string> errors)> ImportFromCsvAsync(
        string csvContent, List<Carrier> carriers, List<OrderTracker.Models.PaymentMethod> paymentMethods)
    {
        var lines = csvContent.Replace("\r\n", "\n").Replace("\r", "\n")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        if (lines.Count < 2)
            return (0, 0, new List<string> { AppText.CsvNoDataMessage });

        int imported = 0, skipped = 0;
        var errors = new List<string>();
        var carrierMap = carriers.ToDictionary(c => c.Name.ToLower(), c => c);
        var pmMap = paymentMethods.ToDictionary(p => p.Name.ToLower(), p => p.Id);

        for (int i = 1; i < lines.Count; i++)
        {
            try
            {
                var cols = ParseCsvLine(lines[i]);
                if (cols.Count < 11) { skipped++; continue; }

                var order = new Order
                {
                    Store = cols[0],
                    PurchaseDate = DateTime.TryParseExact(cols[1], "dd/MM/yyyy", null,
                        System.Globalization.DateTimeStyles.None, out var pd) ? pd : DateTime.Today,
                    TrackingCode = string.IsNullOrWhiteSpace(cols[3]) ? null : cols[3],
                    TrackingUrl = string.IsNullOrWhiteSpace(cols[4]) ? null : cols[4],
                    Status = AppText.ParseOrderStatus(cols[5]),
                    EstimatedDelivery = DateTime.TryParseExact(cols[6], "dd/MM/yyyy", null,
                        System.Globalization.DateTimeStyles.None, out var ed) ? ed : (DateTime?)null,
                    ReceivedDate = DateTime.TryParseExact(cols[7], "dd/MM/yyyy", null,
                        System.Globalization.DateTimeStyles.None, out var rd) ? rd : (DateTime?)null,
                    TotalPrice = decimal.TryParse(cols[8].Replace(",", "."),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var tp) ? tp : 0,
                    Notes = string.IsNullOrWhiteSpace(cols[10]) ? null : cols[10],
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var carrierName = cols[2].ToLower();
                if (!string.IsNullOrWhiteSpace(carrierName) && carrierMap.TryGetValue(carrierName, out var carrier))
                {
                    order.CarrierId = carrier.Id;
                    if (string.IsNullOrWhiteSpace(order.TrackingUrl) && !string.IsNullOrWhiteSpace(order.TrackingCode))
                        order.TrackingUrl = new CarrierService(_db).GetTrackingUrl(carrier, order.TrackingCode);
                }

                var pmName = cols[9].ToLower();
                if (!string.IsNullOrWhiteSpace(pmName) && pmMap.TryGetValue(pmName, out var pmId))
                    order.PaymentMethodId = pmId;

                _db.Orders.Add(order);
                imported++;
            }
            catch (Exception ex)
            {
                errors.Add(AppText.FormatImportLineError(i + 1, ex.Message));
                skipped++;
            }
        }

        if (imported > 0)
            await _db.SaveChangesAsync();

        return (imported, skipped, errors);
    }

    private static void RecalculateTotal(Order order)
    {
        order.TotalPrice = order.Items.Sum(i => i.Quantity * i.UnitPrice);
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        return value;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var inQuotes = false;
        var current = new System.Text.StringBuilder();

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result;
    }
}
