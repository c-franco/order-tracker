using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderTracker.Resources;

namespace OrderTracker.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    [Required(ErrorMessageResourceType = typeof(AppText), ErrorMessageResourceName = nameof(AppText.ValidationProductNameRequired))]
    [MaxLength(300)]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, 9999, ErrorMessageResourceType = typeof(AppText), ErrorMessageResourceName = nameof(AppText.ValidationQuantityRange))]
    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 99999.99, ErrorMessageResourceType = typeof(AppText), ErrorMessageResourceName = nameof(AppText.ValidationUnitPriceRange))]
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal Subtotal => Quantity * UnitPrice;
}
