using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.MySql;

public class AsyncDatabaseFilterNotEqualIntegrationTests(MySqlFixture fixture)
    : GenericNotEqualTest<MySqlFixture>(fixture);