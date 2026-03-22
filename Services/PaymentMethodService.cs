using Microsoft.EntityFrameworkCore;
using OrderTracker.Data;
using OrderTracker.Models;

namespace OrderTracker.Services;

public class PaymentMethodService
{
    private readonly AppDbContext _db;

    public PaymentMethodService(AppDbContext db) => _db = db;

    public async Task<List<PaymentMethod>> GetAllAsync() =>
        await _db.PaymentMethods.OrderBy(p => p.SortOrder).ThenBy(p => p.Name).ToListAsync();

    public async Task SaveAllAsync(List<PaymentMethod> methods)
    {
        var allInDb = await _db.PaymentMethods.ToListAsync();
        var incomingIds = methods.Where(m => m.Id > 0).Select(m => m.Id).ToHashSet();

        foreach (var dbMethod in allInDb)
            if (!incomingIds.Contains(dbMethod.Id))
                _db.PaymentMethods.Remove(dbMethod);

        foreach (var method in methods)
        {
            var existing = await _db.PaymentMethods.FindAsync(method.Id);
            if (existing == null)
                _db.PaymentMethods.Add(method);
            else
            {
                existing.Name = method.Name;
                existing.SortOrder = method.SortOrder;
            }
        }

        await _db.SaveChangesAsync();
    }
}
