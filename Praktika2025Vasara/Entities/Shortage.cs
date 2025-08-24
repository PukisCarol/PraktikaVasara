namespace Praktika2025Vasara.Entities;

public class Shortage
{
    public string Title { get; set; }
    public string Name { get; set; }
    public Room Room { get; set; }
    public Category Category { get; set; }
    public int? Priority { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; }

    public Shortage(string title, string name, Room room, Category category, int? priority, string createdBy)
    {
        Title = title;
        Name = name;
        Room = room;
        Category = category;
        Priority = priority;
        CreatedOn = DateTime.Now;
        CreatedBy = createdBy;
    }
}
