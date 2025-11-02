using Ooze.Typed.EntityFrameworkCore.Extensions;
using Ooze.Typed.Filters;
using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostLikeFiltersProvider : IFilterProvider<Post, PostLikeFilters>,
    IAsyncFilterProvider<Post, PostLikeFilters>
{
    public IEnumerable<FilterDefinition<Post, PostLikeFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostLikeFilters>()
            .Like(post => post.Name, filter => filter.LikeExpr)
            .Build();

    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostLikeFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostLikeFilters>()
            .Like(post => post.Name, filter => filter.LikeExpr)
            .Build());
}