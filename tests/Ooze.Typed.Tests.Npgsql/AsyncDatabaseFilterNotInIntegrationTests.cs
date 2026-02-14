using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Npgsql;

public class AsyncDatabaseFilterNotInThanIntegrationTests(NpgsqlFixture fixture)
    : GenericNotInTest<NpgsqlFixture>(fixture);