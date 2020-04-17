namespace Ooze.Configuration.Options
{
    public class OozeOptions
    {
        public OozeOperations Operations { get; internal set; } = new OozeOperations();
        public bool UseSelections { get; set; } = false;
    }
}
