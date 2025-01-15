using ExampleLibrary.Other;

namespace ExampleLibrary.ViewModel
{
    public class CreateStudentViewModelScopedNamespace : NamedEntity
    {        
        public string Email { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public bool CreateInExternalSystem { get; set; }
    }
}
