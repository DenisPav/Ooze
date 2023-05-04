namespace Ooze.Configuration.Options
{
    public class OozeOptions
    {
        public OozeOperations Operations { get; internal set; } = new OozeOperations();
        public OozePaging Paging { get; internal set; } = new OozePaging();
        public bool UseSelections { get; set; } = false;

    }

    public class OozePaging
    {
        public bool UsePaging { get; set; } = false;
        public int DefaultPageSize { get; set; } = 20;
    }
}
