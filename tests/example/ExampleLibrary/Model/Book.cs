using ExampleLibrary.Model.Base;

namespace ExampleLibrary.Model;

public class Book : ModelEntity
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int NumberOfPages { get; set; }
    public DateOnly PublishedAt { get; set; }
}
