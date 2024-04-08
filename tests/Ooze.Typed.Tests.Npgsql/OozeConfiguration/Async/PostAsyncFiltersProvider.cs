using Ooze.Typed.EntityFrameworkCore.Npgsql.Extensions;
using Ooze.Typed.Filters.Async;

namespace Ooze.Typed.Tests.Npgsql.OozeConfiguration.Async;

public class PostAsyncFiltersProvider : IAsyncFilterProvider<Post, PostFilters>
{
    public ValueTask<IEnumerable<AsyncFilterDefinition<Post, PostFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Post, PostFilters>()
            .Equal(post => post.Id, filter => filter.PostId)
            .NotEqual(post => post.Id, filter => filter.NotEqualPostId)
            .GreaterThan(post => post.Id, filter => filter.GreaterThanPostId)
            .LessThan(post => post.Id, filter => filter.LessThanPostId)
            .In(post => post.Id, filter => filter.IdIn)
            .NotIn(post => post.Id, filter => filter.IdNotIn)
            .Range(post => post.Id, filter => filter.IdRange)
            .OutOfRange(post => post.Id, filter => filter.IdOutOfRange)
            .StartsWith(post => post.Name, filter => filter.NameStartsWith)
            .DoesntStartWith(post => post.Name, filter => filter.NameDoesntWith)
            .EndsWith(post => post.Name, filter => filter.NameEndsWith)
            .DoesntEndWith(post => post.Name, filter => filter.NameDoesntEndWith)
            .InsensitiveLike(post => post.Name, filter => filter.NameLikeFilter)
            .SoundexEqual(post => post.Name, filter => filter.NameSoundexEqual)
            .Build());
}