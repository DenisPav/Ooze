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

public class BlogFilters
{
    public int? IdFilter { get; set; }
    public string NameFilter { get; set; }
    public int? NumberOfCommentsFilter { get; set; }

    public BlogFilters()
    {
        
    }
    
    public BlogFilters(int? idFilter, string nameFilter, int? numberOfCommentsFilter)
    {
        IdFilter = idFilter;
        NameFilter = nameFilter;
        NumberOfCommentsFilter = numberOfCommentsFilter;
    }
}

public record BlogSorters(SortDirection? IdSort, SortDirection? NameSort, SortDirection? NumberOfCommentsSort);