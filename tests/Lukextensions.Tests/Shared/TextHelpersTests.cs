using FluentAssertions;
using Lukextensions.Shared;

namespace Lukextensions.Tests.Shared
{
    public class TextHelpersTests
    {

        [Theory]
        [InlineData("cat", "cats")]
        [InlineData("house", "houses")]
        public void Pluralize_Default_AppendsS(string original, string expected)
        {
            // Arrange

            // Act
            var result = TextHelper.PluralizeListName(original);

            // Assert
            result.Should().Be(expected);
        }


        [Theory]
        [InlineData("iris", "irises")]
        [InlineData("truss", "trusses")]
        [InlineData("marsh", "marshes")]
        [InlineData("lunch", "lunches")]
        [InlineData("tax", "taxes")]
        [InlineData("blitz", "blitzes")]
        public void Pluralize_SpecificEnding_AppendsEs(string original, string expected)
        {
            // Arrange

            // Act
            var result = TextHelper.PluralizeListName(original);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("city", "cities")]
        [InlineData("puppy", "puppies")]
        public void Pluralize_EndingWithConsonantY_AppendsIes(string original, string expected)
        {
            // Arrange

            // Act
            var result = TextHelper.PluralizeListName(original);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("ray", "rays")]
        [InlineData("boy", "boys")]
        public void Pluralize_EndingWithVowelY_AppendsS(string original, string expected)
        {
            // Arrange

            // Act
            var result = TextHelper.PluralizeListName(original);

            // Assert
            result.Should().Be(expected);
        }

    }
}
