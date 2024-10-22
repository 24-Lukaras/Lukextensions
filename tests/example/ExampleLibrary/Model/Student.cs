
namespace ExampleLibrary.Model;

public class Student
{
    public int Id { get; set; }
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string ExternalSystemId { get; set; } = null!;
}
