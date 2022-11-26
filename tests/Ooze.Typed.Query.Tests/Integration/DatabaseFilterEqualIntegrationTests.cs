using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Query.Tests.Integration.Setup;

namespace Ooze.Typed.Query.Tests.Integration
{
    public class DatabaseQueryFilterIntegrationTests : IClassFixture<DbFixture<DatabaseContext>>
    {
        readonly DbFixture<DatabaseContext> _fixture;

        public DatabaseQueryFilterIntegrationTests(DbFixture<DatabaseContext> fixture) => _fixture = fixture;

        [Theory]
        [InlineData("Id == '3'", 1)]
        [InlineData("Id << '3'", 2)]
        [InlineData("Id <= '3'", 3)]
        [InlineData("(Id >> '3')", 97)]
        [InlineData("(Id >= '3')", 98)]
        [InlineData("(Id != '3')", 99)]
        public async Task Should_Correctly_Filter_Data_By_Query_Long_Filter_And_Have_Correct_Count(
            string queryDefinition,
            int expectedRowCount)
        {
            using var scope = _fixture.CreateServiceProvider<PostQueryFiltersProvider>().CreateScope();
            var provider = scope.ServiceProvider;

            await using var context = _fixture.CreateContext();
            var oozeResolver = provider.GetRequiredService<IOozeTypedResolver>();

            IQueryable<Post> query = context.Posts;
            query = oozeResolver.Query(query, queryDefinition);

            var results = await query.ToListAsync();
            Assert.True(results.Count == expectedRowCount);
        }
        
        [Theory]
        [InlineData("Id @= '3'")]
        [InlineData("Id =@ '3'")]
        [InlineData("Id %% '3'")]
        public async Task Should_Fail_If_Operation_Not_Defined_For_Long(string queryDefinition)
        {
            using var scope = _fixture.CreateServiceProvider<PostQueryFiltersProvider>().CreateScope();
            var provider = scope.ServiceProvider;

            await using var context = _fixture.CreateContext();
            var oozeResolver = provider.GetRequiredService<IOozeTypedResolver>();

            IQueryable<Post> query = context.Posts;
            Assert.Throws<ArgumentException>(() => oozeResolver.Query(query, queryDefinition));
        }
    }
}