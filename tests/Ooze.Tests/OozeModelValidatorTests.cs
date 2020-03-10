using Ooze.Validation;
using Xunit;

namespace Ooze.Tests
{
    public class OozeModelValidatorTests
    {
        [Theory]
        [InlineData("", "", false, false)]
        [InlineData(null, null, false, false)]
        [InlineData(" ", " ", false, false)]
        [InlineData("-", "-", true, true)]
        [InlineData("sample", "sample", true, true)]

        public void Should_Correctly_Validate_Model(
            string sorter,
            string filter,
            bool validSorter,
            bool validFilter)
        {
            var context = new OozeModelValidatorContext();
            var model = new OozeModel
            {
                Sorters = sorter,
                Filters = filter
            };

            var (sorterResult, filterResult) = context.OozeModelValidator.Validate(model);

            Assert.True(sorterResult == validSorter);
            Assert.True(filterResult == validFilter);
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
