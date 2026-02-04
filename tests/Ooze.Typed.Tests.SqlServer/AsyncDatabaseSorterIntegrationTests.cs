using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.SqlServer;

public class AsyncDatabaseSorterEqualIntegrationTests(SqlServerFixture fixture)
    : GenericSorterTest<SqlServerFixture>(fixture);