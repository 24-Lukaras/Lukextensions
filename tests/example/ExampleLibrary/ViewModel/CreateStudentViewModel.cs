namespace ExampleLibrary.ViewModel;

public class CreateStudentViewModel
{
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public bool CreateInExternalSystem { get; set; }
}
