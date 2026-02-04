using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.SqlServer;

public class AsyncDatabaseFilterEqualIntegrationTests(SqlServerFixture fixture)
    : GenericEqualsTest<SqlServerFixture>(fixture);