using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Sqlite;

public class AsyncDatabaseFilterOutOfRangeThanIntegrationTests(SqliteFixture fixture)
    : GenericOutOfRangeTest<SqliteFixture>(fixture);