using Ooze.Configuration.Options;
using System.Linq;

namespace Ooze.Validation
{
    internal class OozeOptionsValidator
    {
        public bool Validate(OozeOptions options)
        {
            return ValidateOperations(options.Operations);
        }

        private bool ValidateOperations(OozeOperations operations)
        {
            var fields = operations.GetType()
                .GetFields()
                .ToList();

            var fieldCount = fields.Count;
            var fieldValues = fields.Select(field => field.GetValue(operations).ToString())
                .Distinct()
                .ToList();

            if (fieldCount != fieldValues.Count)
                return false;

            var anyPropHasLettersOrDigits = fieldValues.Any(value => value.Where(@char => !char.IsLetterOrDigit(@char)).Count() != value.Count());
            if (anyPropHasLettersOrDigits)
                return false;

            return true;
        }
    }
}
