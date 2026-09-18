using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Npgsql;

public class AsyncDatabaseFilterEqualIntegrationTests(NpgsqlFixture fixture)
    : GenericEqualsTest<NpgsqlFixture>(fixture);