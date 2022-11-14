using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Sorters;
using Ooze.Typed.Tests.Integration.Setup;

namespace Ooze.Typed.Tests.Integration
{
    public class DatabaseSorterEqualIntegrationTests : IClassFixture<DbFixture<DatabaseContext>>
    {
        readonly DbFixture<DatabaseContext> _fixture;

        public DatabaseSorterEqualIntegrationTests(DbFixture<DatabaseContext> fixture) => _fixture = fixture;

        [Fact]
        public async Task Should_Correctly_Sort_Data_Descending_By_Single_Field()
        {
            using var scope = _fixture.CreateServiceProvider<PostSortersProvider>().CreateScope();
            var provider = scope.ServiceProvider;

            await using var context = _fixture.CreateContext();
            var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters>>();

            var sorters = new[]
            {
                new Sorter("Id", SortDirection.Descending)
            };
            IQueryable<Post> query = context.Posts;
            query = oozeResolver.WithQuery(query)
                .Sort(sorters)
                .Apply();

            var results = await query.ToListAsync();
            Assert.True(results.Count == DatabaseContext.TotalCountOfFakes);
            Assert.True(results[0].Id == 100);
            Assert.True(results[99].Id == 1);
        }
        
        [Fact]
        public async Task Should_Correctly_Sort_Data_Ascending_By_Single_Field()
        {
            using var scope = _fixture.CreateServiceProvider<PostSortersProvider>().CreateScope();
            var provider = scope.ServiceProvider;

            await using var context = _fixture.CreateContext();
            var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters>>();

            var sorters = new[]
            {
                new Sorter("Id", SortDirection.Ascending)
            };
            IQueryable<Post> query = context.Posts;
            query = oozeResolver.WithQuery(query)
                .Sort(sorters)
                .Apply();

            var results = await query.ToListAsync();
            Assert.True(results.Count == DatabaseContext.TotalCountOfFakes);
            Assert.True(results[0].Id == 1);
            Assert.True(results[99].Id == 100);
        }
        
        [Fact]
        public async Task Should_Correctly_Sort_Data_Ascending_By_Multple_Fields()
        {
            using var scope = _fixture.CreateServiceProvider<PostSortersProvider>().CreateScope();
            var provider = scope.ServiceProvider;

            await using var context = _fixture.CreateContext();
            var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters>>();

            var sorters = new[]
            {
                new Sorter("Id", SortDirection.Ascending),
                new Sorter("Enabled", SortDirection.Ascending)
            };
            IQueryable<Post> query = context.Posts;
            query = oozeResolver.WithQuery(query)
                .Sort(sorters)
                .Apply();

            var results = await query.ToListAsync();
            Assert.True(results.Count == DatabaseContext.TotalCountOfFakes);
            Assert.True(results[0].Id == 1);
            Assert.True(results[1].Id == 2);
        }
        
        [Fact]
        public async Task Should_Not_Sort_Data_If_Sort_Not_Provided()
        {
            using var scope = _fixture.CreateServiceProvider<PostSortersProvider>().CreateScope();
            var provider = scope.ServiceProvider;

            await using var context = _fixture.CreateContext();
            var oozeResolver = provider.GetRequiredService<IOozeTypedResolver<Post, PostFilters>>();

            var sorters = Enumerable.Empty<Sorter>();
            IQueryable<Post> query = context.Posts;
            query = oozeResolver.WithQuery(query)
                .Sort(sorters)
                .Apply();

            var results = await query.ToListAsync();
            Assert.True(results.Count == DatabaseContext.TotalCountOfFakes);
            Assert.True(results[0].Id == 1);
            Assert.True(results[99].Id == 100);
        }
    }
}
