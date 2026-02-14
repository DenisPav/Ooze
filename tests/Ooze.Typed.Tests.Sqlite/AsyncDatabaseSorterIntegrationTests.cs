using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Sqlite;

public class AsyncDatabaseSorterEqualIntegrationTests(SqliteFixture fixture)
    : GenericSorterTest<SqliteFixture>(fixture);