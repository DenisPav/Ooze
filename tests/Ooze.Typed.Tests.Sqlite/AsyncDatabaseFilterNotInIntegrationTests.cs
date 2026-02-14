using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Sqlite;

public class AsyncDatabaseFilterNotInThanIntegrationTests(SqliteFixture fixture)
    : GenericNotInTest<SqliteFixture>(fixture);