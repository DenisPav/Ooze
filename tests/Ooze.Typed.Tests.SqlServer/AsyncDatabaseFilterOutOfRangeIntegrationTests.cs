using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.SqlServer;

public class AsyncDatabaseFilterOutOfRangeThanIntegrationTests(SqlServerFixture fixture)
    : GenericOutOfRangeTest<SqlServerFixture>(fixture);