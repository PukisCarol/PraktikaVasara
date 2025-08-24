using Newtonsoft.Json;

namespace Praktika2025Vasara.Store;

public class SimpleFileStore<T> : IFileStore<T>
{
    private List<T> _items;
    private readonly string _filePath;

    public SimpleFileStore(string relativePath)
    {
        _filePath = Path.Combine(AppContext.BaseDirectory, relativePath);
    }

    public List<T> GetItems()
    {
        if (_items == null)
        {
            _items = ReadFromFile();
        }
        return _items;
    }

    public void SaveItems(List<T> items)
    {
        _items = items;
        SaveToFile(items);
    }

    private List<T> ReadFromFile()
    {
        string json = File.ReadAllText(_filePath);
        return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
    }

    private void SaveToFile(List<T> shortages)
    {
        string json = JsonConvert.SerializeObject(shortages, Formatting.Indented);
        File.WriteAllText(_filePath, json);
    }
}