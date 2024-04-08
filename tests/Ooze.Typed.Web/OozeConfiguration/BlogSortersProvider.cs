using Ooze.Typed.Filters;
using Ooze.Typed.Sorters;
using Ooze.Typed.Web.Entities;

public class BlogSortersProvider : ISorterProvider<Blog, BlogSorters>
{
    public IEnumerable<SortDefinition<Blog, BlogSorters>> GetSorters()
    {
        return Sorters.CreateFor<Blog, BlogSorters>()
            .SortBy(blog => blog.Id, sort => sort.BlogIdSort)
            .SortBy(blog => blog.Name, sort => sort.BlogNameSort, _ => false)
            .Build();
    }
}

public class CommentFilters
{
    public string Name { get; set; } = default!;
    public int? CommentId { get; set; } = default!;
    public IEnumerable<int> CommentIds { get; set; } = default!;
    public RangeFilter<int> CommentIdsRange { get; set; } = default!;
}

public class CommentFiltersProvider : IFilterProvider<Comment, CommentFilters>
{
    public IEnumerable<FilterDefinition<Comment, CommentFilters>> GetFilters()
    {
        return Filters.CreateFor<Comment, CommentFilters>()
            .StartsWith(comment => comment.Text, filter => filter.Name)
            .Equal(comment => comment.Id, filter => filter.CommentId)
            .In(comment => comment.Id, filter => filter.CommentIds)
            .Range(comment => comment.Post.Id, filter => filter.CommentIdsRange)
            .Build();
    }
}


public class CommentSorters
{
    public SortDirection? IdSort { get; set; }
    public SortDirection? NameSort { get; set; }
}

public class CommentsSortersProvider : ISorterProvider<Comment, CommentSorters>
{
    public IEnumerable<SortDefinition<Comment, CommentSorters>> GetSorters()
    {
        return Sorters.CreateFor<Comment, CommentSorters>()
            .SortBy(comment => comment.Post.Id, sort => sort.IdSort)
            .SortBy(comment => comment.Text, sort => sort.NameSort)
            .Build();
    }
}