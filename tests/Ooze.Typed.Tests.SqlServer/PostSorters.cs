using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests.SqlServer;

public record class PostSorters(SortDirection? Id, SortDirection? Name, SortDirection? Enabled);