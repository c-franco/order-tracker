using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderTracker.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    [Required(ErrorMessage = "El nombre del producto es obligatorio")]
    [MaxLength(300)]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, 9999, ErrorMessage = "La cantidad debe ser mayor que 0")]
    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 99999.99, ErrorMessage = "El precio debe ser mayor que 0")]
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal Subtotal => Quantity * UnitPrice;
}
