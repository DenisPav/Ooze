using Ooze.Typed.EntityFrameworkCore.Extensions;
using Ooze.Typed.Filters;
using Ooze.Typed.Web.Entities;

public class BlogFiltersProvider : IOozeFilterProvider<Blog, BlogFilters>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BlogFiltersProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public IEnumerable<IFilterDefinition<Blog, BlogFilters>> GetFilters()
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
}
