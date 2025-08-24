using Praktika2025Vasara.Entities;

namespace Praktika2025Vasara.Services;

public interface IShortageService
{
    bool DoesShortageExists(Shortage newShortage);
    void AddShortage(Shortage newShortage);
    bool DeleteShortage(string title, Room room, string username, bool isAdmin);

    List<Shortage> FindShortages(string username, bool isAdmin, string titleFilter = null, DateTime? fromDate = null,
        DateTime? toDate = null, Category? categoryFilter = null, Room? roomFilter = null);
}