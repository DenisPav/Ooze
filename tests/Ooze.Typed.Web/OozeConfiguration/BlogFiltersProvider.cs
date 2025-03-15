using Ooze.Typed.EntityFrameworkCore.Extensions;
using Ooze.Typed.Filters;
using Ooze.Typed.Filters.Async;
using Ooze.Typed.Web.Entities;

public class BlogFiltersProvider :
    IFilterProvider<Blog, BlogFilters>,
    IAsyncFilterProvider<Blog, BlogFilters>,
    IQueryFilterProvider<Blog>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BlogFiltersProvider(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IEnumerable<FilterDefinition<Blog, BlogFilters>> GetFilters()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var hasSecretParam = !httpContext?.Request.Query.ContainsKey("secret") ?? true;

        return Filters.CreateFor<Blog, BlogFilters>()
            .Equal(blog => blog.Id, filter => filter.BlogId)
            .Range(blog => blog.Id, filter => filter.BlogRange, _ => hasSecretParam)
            .In(blog => blog.Id, filter => filter.BlogIds, filters => false)
            .Like(blog => blog.Name, filter => filter.Name)
            .Build();
    }

    public ValueTask<IEnumerable<AsyncFilterDefinition<Blog, BlogFilters>>> GetFiltersAsync()
    {
        var filters = AsyncFilters.CreateFor<Blog, BlogFilters>()
            .Add(filter => string.IsNullOrEmpty(filter.Name) == false, filter => blog => blog.Name == filter.Name)
            .AddAsync(async filter =>
            {
                await Task.Delay(1);
                return string.IsNullOrEmpty(filter.Name) == false;
            },
            async filter =>
            {
                await Task.Delay(1);
                return blog => blog.Name == filter.Name;
            })
            // .Add(filter => string.IsNullOrEmpty(filter.Query) == false, filter => _blogQueryHandler.Apply(null, filter.Query))
            .Build();

        return ValueTask.FromResult(filters);
    }

    public IEnumerable<QueryFilterDefinition<Blog>> GetMappings()
    {
        return QueryFilters.CreateFor<Blog>()
            .Add(x => x.Name, "neeejm")
            .Build();
    }
}
