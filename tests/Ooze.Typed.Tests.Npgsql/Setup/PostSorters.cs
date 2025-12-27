using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests.Npgsql.Setup;

public record PostSorters(SortDirection? Id, SortDirection? Name, SortDirection? Enabled);