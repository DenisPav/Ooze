namespace Ooze.Validation
{
    internal class OozeModelValidator
    {
        public OozeModelValidationResult Validate(OozeModel model)
        {
            var sortersValid = !string.IsNullOrEmpty(model.Sorters) && !string.IsNullOrWhiteSpace(model.Sorters);
            var filtersValid = !string.IsNullOrEmpty(model.Filters) && !string.IsNullOrWhiteSpace(model.Filters);
            var queryValid = !string.IsNullOrEmpty(model.Query) && !string.IsNullOrWhiteSpace(model.Query);
            var fieldsValid = !string.IsNullOrEmpty(model.Fields) && !string.IsNullOrWhiteSpace(model.Fields);

            return new OozeModelValidationResult(sortersValid, filtersValid, queryValid, fieldsValid);
        }
    }
}
