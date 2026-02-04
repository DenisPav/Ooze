using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.SqlServer;

public class AsyncDatabaseFilterLessThanIntegrationTests(SqlServerFixture fixture)
    : GenericLessThanTest<SqlServerFixture>(fixture);