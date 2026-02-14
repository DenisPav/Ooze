using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.MySql;

public class AsyncDatabaseFilterNotInThanIntegrationTests(MySqlFixture fixture)
    : GenericNotInTest<MySqlFixture>(fixture);