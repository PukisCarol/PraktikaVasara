using Praktika2025Vasara.Entities;
using Praktika2025Vasara.Store;

namespace Praktika2025Vasara.Services;

public class ShortageService : IShortageService
{
    private readonly IFileStore<Shortage> _store;

    public ShortageService(IFileStore<Shortage> store)
    {
        _store = store;
    }

    public bool DoesShortageExists(Shortage newShortage)
    {
        List<Shortage> shortages = _store.GetItems();
        Shortage existing = shortages.FirstOrDefault(s =>
            s.Title.Equals(newShortage.Title, StringComparison.OrdinalIgnoreCase) &&
            s.Room == newShortage.Room);

        if (existing != null && newShortage.Priority > existing.Priority)
        {
            return true;
        }
        return false;
    }

    public void AddShortage(Shortage newShortage)
    {
        List<Shortage> shortages = _store.GetItems();
        shortages.Add(newShortage);
        _store.SaveItems(shortages);
    }

    public bool DeleteShortage(string title, Room room, string username, bool isAdmin)
    {
        List<Shortage> shortages = _store.GetItems();
        Shortage shortage = shortages.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase) && s.Room == room);

        if (shortage == null)
        {
            throw new InvalidOperationException("Shortage not found.");
        }

        if (shortage.CreatedBy != username && !isAdmin)
        {
            throw new InvalidOperationException("Only the creator or an administrator can delete this shortage.");
        }

        shortages.Remove(shortage);
        _store.SaveItems(shortages);

        return true;
    }

    public List<Shortage> FindShortages(string username, bool isAdmin, string titleFilter = null, DateTime? fromDate = null,
        DateTime? toDate = null, Category? categoryFilter = null, Room? roomFilter = null)
    {
        List<Shortage> shortages = _store.GetItems();
        IEnumerable<Shortage> query = shortages;

        if (!isAdmin)
        {
            query = query.Where(u => u.CreatedBy == username);
        }

        if (!string.IsNullOrWhiteSpace(titleFilter))
        {
            query = query.Where(u => u.Title.IndexOf(titleFilter, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(u => u.CreatedOn >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(u => u.CreatedOn <= toDate.Value);
        }

        if (categoryFilter.HasValue)
        {
            query = query.Where(u => u.Category == categoryFilter.Value);
        }

        if (roomFilter.HasValue)
        {
            query = query.Where(u => u.Room == roomFilter.Value);
        }

        return query.OrderByDescending(u => u.Priority).ToList();
    }
}
