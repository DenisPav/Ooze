using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.MySql;

public class AsyncDatabaseFilterRangeThanIntegrationTests(MySqlFixture fixture)
    : GenericRangeTest<MySqlFixture>(fixture);