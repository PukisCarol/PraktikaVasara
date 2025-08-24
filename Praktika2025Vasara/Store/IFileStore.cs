namespace Praktika2025Vasara.Store;

public interface IFileStore<T>
{
    List<T> GetItems();
    void SaveItems(List<T> items);
}