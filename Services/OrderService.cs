using Microsoft.EntityFrameworkCore;
using OrderTracker.Data;
using OrderTracker.DTOs;
using OrderTracker.Models;

namespace OrderTracker.Services;

public class OrderService
{
    private readonly AppDbContext _db;
    private readonly ShippingDetectionService _shipping;

    public OrderService(AppDbContext db, ShippingDetectionService shipping)
    {
        _db = db;
        _shipping = shipping;
    }

    public async Task<List<Order>> GetAllAsync(string? search = null, OrderStatus? statusFilter = null)
    {
        var query = _db.Orders
            .Include(o => o.Items)
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
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        // Auto-detect shipping company
        if (!string.IsNullOrWhiteSpace(order.TrackingCode) && order.ShippingCompany == ShippingCompany.Desconocido)
        {
            var (company, url) = _shipping.DetectShipping(order.TrackingCode);
            order.ShippingCompany = company;
            if (string.IsNullOrWhiteSpace(order.TrackingUrl))
                order.TrackingUrl = url;
        }
        else if (!string.IsNullOrWhiteSpace(order.TrackingCode) && string.IsNullOrWhiteSpace(order.TrackingUrl))
        {
            order.TrackingUrl = _shipping.GetTrackingUrl(order.ShippingCompany, order.TrackingCode);
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
        existing.ShippingCompany = order.ShippingCompany;
        existing.TrackingCode = order.TrackingCode;
        existing.Status = order.Status;
        existing.EstimatedDelivery = order.EstimatedDelivery;
        existing.Notes = order.Notes;
        existing.UpdatedAt = DateTime.Now;

        // Auto-detect or generate tracking URL
        if (!string.IsNullOrWhiteSpace(order.TrackingCode))
        {
            if (order.ShippingCompany == ShippingCompany.Desconocido)
            {
                var (company, url) = _shipping.DetectShipping(order.TrackingCode);
                existing.ShippingCompany = company;
                existing.TrackingUrl = string.IsNullOrWhiteSpace(order.TrackingUrl) ? url : order.TrackingUrl;
            }
            else
            {
                existing.TrackingUrl = string.IsNullOrWhiteSpace(order.TrackingUrl)
                    ? _shipping.GetTrackingUrl(order.ShippingCompany, order.TrackingCode)
                    : order.TrackingUrl;
            }
        }
        else
        {
            existing.TrackingUrl = null;
        }

        // Replace items
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
                o.EstimatedDelivery.Value < now &&
                o.Status != OrderStatus.Recibido &&
                o.Status != OrderStatus.Cancelado),
            TotalSpent = orders
                .Where(o => o.Status != OrderStatus.Cancelado)
                .Sum(o => o.TotalPrice)
        };
    }

    private static void RecalculateTotal(Order order)
    {
        order.TotalPrice = order.Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
