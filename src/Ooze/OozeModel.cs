namespace Ooze
{
    public class OozeModel
    {
        public string Sorters { get; set; }
        public string Filters { get; set; }
        public string Query { get; set; }
        public string Fields { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
