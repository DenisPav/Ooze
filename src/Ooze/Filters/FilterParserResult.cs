namespace Ooze.Filters
{
    public class FilterParserResult
    {
        public string Property { get; set; }
        public string Operation { get; set; }
        public string Value { get; set; }

        public FilterParserResult(string property, string operation, string value)
        {
            Property = property;
            Operation = operation;
            Value = value;
        }
    }
}
