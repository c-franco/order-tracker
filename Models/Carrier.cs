using System.ComponentModel.DataAnnotations;
using OrderTracker.Resources;

namespace OrderTracker.Models;

public class Carrier
{
    public int Id { get; set; }

    [Required(ErrorMessageResourceType = typeof(AppText), ErrorMessageResourceName = nameof(AppText.ValidationCarrierNameRequired))]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? TrackingUrlTemplate { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; } = 0;
}
