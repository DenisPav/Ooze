using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.MySql;

public class AsyncDatabaseFilterLessThanIntegrationTests(MySqlFixture fixture)
    : GenericLessThanTest<MySqlFixture>(fixture);