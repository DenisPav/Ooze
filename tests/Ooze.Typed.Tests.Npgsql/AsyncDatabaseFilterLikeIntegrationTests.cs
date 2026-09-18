using Ooze.Typed.Tests.Base;

namespace Ooze.Typed.Tests.Npgsql;

public class AsyncDatabaseFilterLikeIntegrationTests(NpgsqlFixture fixture)
    : GenericLikeTest<NpgsqlFixture>(fixture);