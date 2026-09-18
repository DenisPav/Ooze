using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Npgsql;

public class AsyncDatabaseFilterGreaterThanIntegrationTests(NpgsqlFixture fixture)
    : GenericGreaterThanTest<NpgsqlFixture>(fixture);