using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.SqlServer;

public class AsyncDatabaseFilterLikeIntegrationTests(SqlServerFixture fixture)
    : GenericLikeTest<SqlServerFixture>(fixture);