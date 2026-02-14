using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Npgsql;

public class AsyncDatabaseFilterNotEqualIntegrationTests(NpgsqlFixture fixture)
    : GenericNotEqualTest<NpgsqlFixture>(fixture);