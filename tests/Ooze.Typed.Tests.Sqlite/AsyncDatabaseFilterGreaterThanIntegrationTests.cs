using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Sqlite;

public class AsyncDatabaseFilterGreaterThanIntegrationTests(SqliteFixture fixture)
    : GenericGreaterThanTest<SqliteFixture>(fixture);