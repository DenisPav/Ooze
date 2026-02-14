using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Sqlite;

public class AsyncDatabaseFilterEqualIntegrationTests(SqliteFixture fixture)
    : GenericEqualsTest<SqliteFixture>(fixture);