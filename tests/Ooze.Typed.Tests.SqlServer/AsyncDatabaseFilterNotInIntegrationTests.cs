using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.SqlServer;

public class AsyncDatabaseFilterNotInThanIntegrationTests(SqlServerFixture fixture)
    : GenericNotInTest<SqlServerFixture>(fixture);