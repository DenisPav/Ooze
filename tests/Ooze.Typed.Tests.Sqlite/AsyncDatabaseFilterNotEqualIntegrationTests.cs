using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Sqlite;

public class AsyncDatabaseFilterNotEqualIntegrationTests(SqliteFixture fixture)
    : GenericNotEqualTest<SqliteFixture>(fixture);