namespace Ooze.Typed.Sorters
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public record class Sorter(string Name, SortDirection Direction);
}
