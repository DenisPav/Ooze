using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.MySql;

public class AsyncDatabaseSorterEqualIntegrationTests(MySqlFixture fixture)
    : GenericSorterTest<MySqlFixture>(fixture);
