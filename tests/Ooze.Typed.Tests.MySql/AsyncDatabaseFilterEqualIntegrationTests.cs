using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.MySql;

public class AsyncDatabaseFilterEqualIntegrationTests(MySqlFixture fixture)
    : GenericEqualsTest<MySqlFixture>(fixture);