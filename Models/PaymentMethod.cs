using System.ComponentModel.DataAnnotations;

namespace OrderTracker.Models;

public class PaymentMethod
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; } = 0;
}
