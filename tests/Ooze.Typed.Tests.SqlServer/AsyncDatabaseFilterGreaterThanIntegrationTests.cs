using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.SqlServer;

public class AsyncDatabaseFilterGreaterThanIntegrationTests(SqlServerFixture fixture)
    : GenericGreaterThanTest<SqlServerFixture>(fixture);