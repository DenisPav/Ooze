using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.SqlServer;

public class AsyncDatabaseFilterRangeThanIntegrationTests(SqlServerFixture fixture)
    : GenericRangeTest<SqlServerFixture>(fixture);