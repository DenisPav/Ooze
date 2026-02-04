using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Npgsql;

public class AsyncDatabaseSorterEqualIntegrationTests(NpgsqlFixture fixture)
    : GenericSorterTest<NpgsqlFixture>(fixture);