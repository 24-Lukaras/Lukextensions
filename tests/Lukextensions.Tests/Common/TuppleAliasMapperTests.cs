
using FluentAssertions;
using Lukextensions.Common;
using Xunit.Abstractions;

namespace Lukextensions.Tests.Common;

public class TuppleAliasMapperTests
{
    private readonly ITestOutputHelper _output;
    public TuppleAliasMapperTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task TuppleAliasMapper_StudentMapper_ShouldCreateMapper()
    {
        // arrange
        string originalContent;
        string expectedContent;
        using (var reader = new StreamReader(Path.Combine(Paths.ExampleProjectRootPath, "Mappers/Student/StudentMapper.cs")))
        {
            originalContent = await reader.ReadToEndAsync();
        }
        using (var reader = new StreamReader(Path.Combine(Paths.ExampleProjectRootPath, "Mappers/Student/Expected.txt")))
        {
            expectedContent = await reader.ReadToEndAsync();
        }
        var compilationProvider = new RecursiveCompilationProvider(Paths.ExampleProjectRootPath);
        var mapperBuilder = new MapperBuilder(originalContent, compilationProvider);

        // act
        var result = await mapperBuilder.Process();

        // assert
        result.Should().Be(expectedContent);
    }

}
