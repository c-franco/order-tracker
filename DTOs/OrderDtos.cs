using OrderTracker.Models;

namespace OrderTracker.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string Store { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public string? TrackingCode { get; set; }
    public string? TrackingUrl { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime? EstimatedDelivery { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}

public class DashboardMetrics
{
    public int TotalPending { get; set; }
    public int InDelivery { get; set; }
    public int Delayed { get; set; }
    public decimal TotalSpent { get; set; }
    public int TotalOrders { get; set; }
}

public class CarrierDeliveryStats
{
    public string CarrierName { get; set; } = string.Empty;
    public double AvgDays { get; set; }
    public int OrderCount { get; set; }
}
