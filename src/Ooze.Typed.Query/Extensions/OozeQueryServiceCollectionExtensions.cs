using Ooze.Typed.Extensions;

namespace Ooze.Typed.Query.Extensions;

public static class OozeQueryServiceCollectionExtensions
{
    public static IOozeServiceCollectionBuilder AddQueryHandler(this IOozeServiceCollectionBuilder oozeBuilder)
        => oozeBuilder.AddQueryHandler(typeof(OozeQueryHandler<>));
}