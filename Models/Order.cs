using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderTracker.Resources;

namespace OrderTracker.Models;

public enum OrderStatus
{
    Comprado,
    Enviado,
    EnReparto,
    Incidencia,
    Recibido,
    Cancelado
}

public class Order
{
    public int Id { get; set; }

    [Required(ErrorMessageResourceType = typeof(AppText), ErrorMessageResourceName = nameof(AppText.ValidationOrderStoreRequired))]
    [MaxLength(200)]
    public string Store { get; set; } = string.Empty;

    [Required(ErrorMessageResourceType = typeof(AppText), ErrorMessageResourceName = nameof(AppText.ValidationPurchaseDateRequired))]
    public DateTime PurchaseDate { get; set; } = DateTime.Now;

    public int? CarrierId { get; set; }
    public Carrier? Carrier { get; set; }

    [MaxLength(100)]
    public string? TrackingCode { get; set; }

    public string? TrackingUrl { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Comprado;

    public DateTime? EstimatedDelivery { get; set; }

    public DateTime? ReceivedDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    public int? PaymentMethodId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
