using Ooze.Typed.EntityFrameworkCore.Npgsql.Extensions;
using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.Npgsql.Setup.Async;

public class AsyncPostPsqlFiltersProvider : IAsyncFilterProvider<Post, PostPsqlFilters>
{
    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostPsqlFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostPsqlFilters>()
            .InsensitiveLike(post => post.Name, filter => filter.ILikeFilter)
            .SoundexEqual(post => post.Name, filter => filter.SoundexFilter)
            .Build());
}