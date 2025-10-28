using Ooze.Typed.EntityFrameworkCore.Sqlite.Extensions;
using Ooze.Typed.Filters;
using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostGlobFiltersProvider : IFilterProvider<Post, PostGlobFilters>,
    IAsyncFilterProvider<Post, PostGlobFilters>
{
    public IEnumerable<FilterDefinition<Post, PostGlobFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostGlobFilters>()
            .Glob(post => post.Name, filter => filter.GlobId)
            .Build();

    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostGlobFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostGlobFilters>()
            .Glob(post => post.Name, filter => filter.GlobId)
            .Build());
}