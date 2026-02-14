using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Sqlite;

public class AsyncDatabaseFilterLessThanIntegrationTests(SqliteFixture fixture)
    : GenericLessThanTest<SqliteFixture>(fixture);