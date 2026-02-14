using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Sqlite;

public class AsyncDatabaseFilterLikeIntegrationTests(SqliteFixture fixture)
    : GenericLikeTest<SqliteFixture>(fixture);