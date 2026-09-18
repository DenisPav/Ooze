using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.MySql;

public class AsyncDatabaseFilterLikeIntegrationTests(MySqlFixture fixture)
    : GenericLikeTest<MySqlFixture>(fixture);