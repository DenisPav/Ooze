using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.SqlServer;

public class AsyncDatabaseFilterNotEqualIntegrationTests(SqlServerFixture fixture)
    : GenericNotEqualTest<SqlServerFixture>(fixture);