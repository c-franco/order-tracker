using Microsoft.EntityFrameworkCore;
using OrderTracker.Data;
using OrderTracker.Models;

namespace OrderTracker.Services;

public class CarrierService
{
    private readonly AppDbContext _db;

    public CarrierService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Carrier>> GetAllAsync()
    {
        return await _db.Carriers
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Carrier?> GetByIdAsync(int id)
    {
        return await _db.Carriers.FindAsync(id);
    }

    public async Task SaveAllAsync(List<Carrier> carriers)
    {
        var allInDb = await _db.Carriers.ToListAsync();
        var incomingIds = carriers.Where(c => c.Id > 0).Select(c => c.Id).ToHashSet();

        // Delete carriers removed from the list
        foreach (var dbCarrier in allInDb)
        {
            if (!incomingIds.Contains(dbCarrier.Id))
                _db.Carriers.Remove(dbCarrier);
        }

        // Add or update
        foreach (var carrier in carriers)
        {
            var existing = await _db.Carriers.FindAsync(carrier.Id);
            if (existing == null)
            {
                _db.Carriers.Add(carrier);
            }
            else
            {
                existing.Name = carrier.Name;
                existing.TrackingUrlTemplate = carrier.TrackingUrlTemplate;
                existing.IsActive = carrier.IsActive;
                existing.SortOrder = carrier.SortOrder;
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task AddAsync(Carrier carrier)
    {
        _db.Carriers.Add(carrier);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var carrier = await _db.Carriers.FindAsync(id);
        if (carrier != null)
        {
            _db.Carriers.Remove(carrier);
            await _db.SaveChangesAsync();
        }
    }

    public string? GetTrackingUrl(Carrier? carrier, string? trackingCode)
    {
        if (carrier == null || string.IsNullOrWhiteSpace(carrier.TrackingUrlTemplate) || string.IsNullOrWhiteSpace(trackingCode))
            return null;
        return string.Format(carrier.TrackingUrlTemplate, trackingCode.Trim());
    }
}
