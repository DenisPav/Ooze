namespace Ooze.Validation
{
    internal class OozeModelValidationResult
    {
        public bool SortersValid { get; private set; }
        public bool FiltersValid { get; private set; }
        public bool QueryValid { get; private set; }
        public bool FieldsValid { get; private set; }

        public OozeModelValidationResult(
            bool sortersValid,
            bool filtersValid,
            bool queryValid,
            bool fieldsValid)
        {
            SortersValid = sortersValid;
            FiltersValid = filtersValid;
            QueryValid = queryValid;
            FieldsValid = fieldsValid;
        }
    }
}
