using Microsoft.EntityFrameworkCore;
using OrderTracker.Data;
using OrderTracker.DTOs;
using OrderTracker.Models;

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
        var now = DateTime.Now;

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
            .GroupBy(o => o.Carrier?.Name ?? "Desconocido")
            .Select(g => new CarrierDeliveryStats
            {
                CarrierName = g.Key,
                AvgDays = g.Average(o => (o.ReceivedDate!.Value.Date - o.PurchaseDate.Date).TotalDays),
                OrderCount = g.Count()
            })
            .OrderBy(s => s.AvgDays)
            .ToList();
    }

    private static void RecalculateTotal(Order order)
    {
        order.TotalPrice = order.Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
