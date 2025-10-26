using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests;

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int NumberOfComments { get; set; }
    public DateTime CreatedAt { get; set; }

    public Blog()
    {
        
    }
    
    public Blog(
        int id,
        string name,
        int numberOfComments,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        NumberOfComments = numberOfComments;
        CreatedAt = createdAt;
    }
}

public record BlogFilters(int? IdFilter, string NameFilter, int? NumberOfCommentsFilter);

public record BlogSorters(SortDirection? IdSort, SortDirection? NameSort, SortDirection? NumberOfCommentsSort);