using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Npgsql;

public class AsyncDatabaseFilterLessThanIntegrationTests(NpgsqlFixture fixture)
    : GenericLessThanTest<NpgsqlFixture>(fixture);