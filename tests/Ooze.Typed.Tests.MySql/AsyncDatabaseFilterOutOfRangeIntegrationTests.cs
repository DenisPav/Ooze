using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.MySql;

public class AsyncDatabaseFilterOutOfRangeThanIntegrationTests(MySqlFixture fixture)
    : GenericOutOfRangeTest<MySqlFixture>(fixture);