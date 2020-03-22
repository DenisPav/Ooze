using Ooze.Validation;
using Xunit;

namespace Ooze.Tests
{
    public class OozeModelValidatorTests
    {
        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData(" ", false)]
        [InlineData("-", true)]
        [InlineData("sample", true)]

        public void Should_Correctly_Validate_Model(
            string param,
            bool isValid)
        {
            var context = new OozeModelValidatorContext();
            var model = new OozeModel
            {
                Sorters = param,
                Filters = param,
                Query = param
            };

            var (sorterResult, filterResult, queryResult) = context.OozeModelValidator.Validate(model);

            Assert.True(sorterResult == isValid);
            Assert.True(filterResult == isValid);
            Assert.True(queryResult == isValid);
        }

        internal class OozeModelValidatorContext
        {
            public OozeModelValidator OozeModelValidator { get; set; }

            public OozeModelValidatorContext()
            {
                OozeModelValidator = new OozeModelValidator();
            }
        }
    }
}
