using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderTracker.Models;

public enum OrderStatus
{
    Comprado,
    PreparandoEnvio,
    Enviado,
    EnReparto,
    Incidencia,
    Recibido,
    Cancelado
}

public enum ShippingCompany
{
    Desconocido,
    Correos,
    CorreosExpress,
    SEUR,
    MRW,
    DHL,
    GLS,
    UPS,
    FedEx,
    AmazonLogistics,
    Nacex,
    Ctt,
    Zeleris,
    EcoScooting,
    Otro
}

public class Order
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La tienda es obligatoria")]
    [MaxLength(200)]
    public string Store { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de compra es obligatoria")]
    public DateTime PurchaseDate { get; set; } = DateTime.Now;

    public ShippingCompany ShippingCompany { get; set; } = ShippingCompany.Desconocido;

    [MaxLength(100)]
    public string? TrackingCode { get; set; }

    public string? TrackingUrl { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Comprado;

    public DateTime? EstimatedDelivery { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
