using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Npgsql;

public class AsyncDatabaseFilterRangeThanIntegrationTests(NpgsqlFixture fixture)
    : GenericRangeTest<NpgsqlFixture>(fixture);