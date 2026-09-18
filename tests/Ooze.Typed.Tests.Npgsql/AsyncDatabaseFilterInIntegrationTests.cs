using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Npgsql;

public class AsyncDatabaseFilterInThanIntegrationTests(NpgsqlFixture fixture)
    : GenericInTest<NpgsqlFixture>(fixture);