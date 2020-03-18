namespace Ooze.Validation
{
    internal class OozeModelValidator
    {
        public (bool sortersValid, bool filtersValid, bool queryValid) Validate(OozeModel model)
        {
            var sortersValid = !string.IsNullOrEmpty(model.Sorters) && !string.IsNullOrWhiteSpace(model.Sorters);
            var filtersValid = !string.IsNullOrEmpty(model.Filters) && !string.IsNullOrWhiteSpace(model.Filters);
            var queryValid = !string.IsNullOrEmpty(model.Query) && !string.IsNullOrWhiteSpace(model.Query);

            return (sortersValid, filtersValid, queryValid);
        }
    }
}
