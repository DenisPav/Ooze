using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Sqlite;

public class AsyncDatabaseFilterRangeIntegrationTests(SqliteFixture fixture)
    : GenericRangeTest<SqliteFixture>(fixture);