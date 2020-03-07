namespace Ooze.Validation
{
    internal class OozeModelValidator
    {
        public (bool sortersValid, bool filtersValid) Validate(OozeModel model)
        {
            var sortersValid = !string.IsNullOrEmpty(model.Sorters) && !string.IsNullOrWhiteSpace(model.Sorters);
            var filtersValid = !string.IsNullOrEmpty(model.Filters) && !string.IsNullOrWhiteSpace(model.Filters);

            return (sortersValid, filtersValid);
        }
    }
}
