using Ooze.Typed.EntityFrameworkCore.Npgsql.Extensions;
using Ooze.Typed.Filters;
using Ooze.Typed.Filters.Async;
using Ooze.Typed.Tests.Base.Setup;

namespace Ooze.Typed.Tests.Npgsql.Setup;

public class PsqlPostFiltersProvider : IFilterProvider<Post, PostPsqlFilters>,
    IAsyncFilterProvider<Post, PostPsqlFilters>
{
    public IEnumerable<FilterDefinition<Post, PostPsqlFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostPsqlFilters>()
            .InsensitiveLike(post => post.Name, filter => filter.ILikeFilter)
            .SoundexEqual(post => post.Name, filter => filter.SoundexFilter)
            .Build();
    
    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostPsqlFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostPsqlFilters>()
            .InsensitiveLike(post => post.Name, filter => filter.ILikeFilter)
            .SoundexEqual(post => post.Name, filter => filter.SoundexFilter)
            .Build());
}

public record PostPsqlFilters(string? ILikeFilter, string? SoundexFilter);