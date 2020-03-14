using Ooze.Configuration.Options;
using Ooze.Validation;
using Xunit;

namespace Ooze.Tests
{
    public class OozeOptionsValidatorTests
    {
        [Fact]
        public void Should_Be_Correctly_Configured()
        {
            var context = new OozeOptionsValidatorContext();
            var options = new OozeOptions
            {
                Operations =
                {
                    Contains = ".",
                    EndsWith = "#",
                    Equal = "$",
                    GreaterThan = "%",
                    GreaterThanOrEqual = "/",
                    LessThan = "(",
                    LessThanOrEqual = ")",
                    NotEqual = "*",
                    StartsWith = "?",
                }
            };

            Assert.True(context.OozeOptionsValidator.Validate(options));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("043.434")]
        [InlineData("ds")]
        [InlineData("GD")]
        public void Should_Not_Be_Correctly_Configured(string operation)
        {
            var context = new OozeOptionsValidatorContext();
            var options = new OozeOptions
            {
                Operations =
                {
                    Contains = operation,
                }
            };

            Assert.False(context.OozeOptionsValidator.Validate(options));
        }

        [Fact]
        public void Should_Not_Have_Same_Operation_String()
        {
            var context = new OozeOptionsValidatorContext();
            var options = new OozeOptions
            {
                Operations =
                {
                    Contains = ".",
                    EndsWith = ".",
                }
            };

            Assert.False(context.OozeOptionsValidator.Validate(options));
        }

        class OozeOptionsValidatorContext
        {
            public OozeOptionsValidator OozeOptionsValidator { get; set; }

            public OozeOptionsValidatorContext()
            {
                OozeOptionsValidator = new OozeOptionsValidator();
            }
        }
    }
}
