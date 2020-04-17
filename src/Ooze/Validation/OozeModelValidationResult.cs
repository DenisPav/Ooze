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

        public void Deconstruct(
            out bool sortersValid,
            out bool filtersValid,
            out bool queryValid,
            out bool fieldsValid)
        {
            sortersValid = SortersValid;
            filtersValid = FiltersValid;
            queryValid = QueryValid;
            fieldsValid = FieldsValid;
        }
    }
}
