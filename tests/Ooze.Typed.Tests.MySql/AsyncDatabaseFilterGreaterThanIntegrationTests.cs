using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.MySql;

public class AsyncDatabaseFilterGreaterThanIntegrationTests(MySqlFixture fixture)
    : GenericGreaterThanTest<MySqlFixture>(fixture);