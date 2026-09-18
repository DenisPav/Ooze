using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Npgsql;

public class AsyncDatabaseFilterOutOfRangeThanIntegrationTests(NpgsqlFixture fixture)
    : GenericOutOfRangeTest<NpgsqlFixture>(fixture);