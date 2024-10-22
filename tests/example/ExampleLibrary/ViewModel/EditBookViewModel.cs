namespace ExampleLibrary.ViewModel;

public class EditBookViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int NumberOfPages { get; set; }
    public DateOnly PublishedAt { get; set; }
    public List<string> Editors { get; set; } = new List<string>();
}
