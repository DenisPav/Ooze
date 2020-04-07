namespace Ooze.Validation
{
    internal class OozeModelValidator
    {
        public (bool sortersValid, bool filtersValid, bool queryValid, bool fieldsValid) Validate(OozeModel model, bool usesSelections)
        {
            var sortersValid = !string.IsNullOrEmpty(model.Sorters) && !string.IsNullOrWhiteSpace(model.Sorters);
            var filtersValid = !string.IsNullOrEmpty(model.Filters) && !string.IsNullOrWhiteSpace(model.Filters);
            var queryValid = !string.IsNullOrEmpty(model.Query) && !string.IsNullOrWhiteSpace(model.Query);
            var fieldsValid = usesSelections ? !string.IsNullOrEmpty(model.Fields) && !string.IsNullOrWhiteSpace(model.Fields) : true;

            return (sortersValid, filtersValid, queryValid, fieldsValid);
        }
    }
}
