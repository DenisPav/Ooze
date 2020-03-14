using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Configuration;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Ooze.Tests.Integration
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Post> Posts { get; set; }

        public async Task Prepare()
        {
            await Database.EnsureCreatedAsync();
            Posts.RemoveRange(Posts);

            var posts = Enumerable.Range(1, 100)
                    .Select(id => new Post
                    {
                        Id = id,
                        Enabled = id % 2 == 0,
                        Name = id.ToString()
                    });

            Posts.AddRange(posts);
            await SaveChangesAsync();
        }
    }
    public class Post
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }

    public class PostConfiguration : IOozeConfiguration
    {
        public void Configure(IOozeConfigurationBuilder builder)
        {
            builder.Entity<Post>()
                 .Sort(post => post.Enabled)
                 .Sort(post => post.Name)
                 .Sort("id2", post => post.Id)
                 .Filter(post => post.Id)
                 .Filter(post => post.Name)
                 .Filter("bool", post => post.Enabled);
        }
    }

    public class DatabaseIntegrationTests : IClassFixture<DbFixture<DatabaseContext>>
    {
        readonly DbFixture<DatabaseContext> _fixture;

        public DatabaseIntegrationTests(
            DbFixture<DatabaseContext> fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("Id>50", 50)]
        [InlineData("bool==True", 50)]
        [InlineData("bool==False", 50)]
        [InlineData("Id==3", 1)]
        [InlineData("Id<=0", 0)]
        [InlineData("Name==3", 1)]
        [InlineData("Id>=10", 91)]
        [InlineData("Id<10", 9)]
        [InlineData("Id!=10", 99)]
        [InlineData("Name@1", 20)]
        [InlineData("Name@=1", 12)]
        [InlineData("Name=@1", 10)]
        public async Task Should_Correctly_Filter_Context_Data(string filter, int expectedCount)
        {
            using var scope = _fixture.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            var oozeResolver = provider.GetRequiredService<IOozeResolver>();

            IQueryable<Post> query = context.Posts;
            query = oozeResolver.Apply(query, new OozeModel { Filters = filter });

            var results = await query.ToListAsync();
            Assert.True(results.Count == expectedCount);
        }

        [Theory]
        [InlineData("Id@3")]
        [InlineData("bool@=3")]
        [InlineData("bool=@3")]
        [InlineData("Name>3")]
        [InlineData("Name>=3")]
        [InlineData("Name<3")]
        [InlineData("Name<=3")]
        public void Should_Fail_To_Filter_Context_Data(string filter)
        {
            using var scope = _fixture.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            var oozeResolver = provider.GetRequiredService<IOozeResolver>();

            IQueryable<Post> query = context.Posts;

            Assert.ThrowsAny<Exception>(() => oozeResolver.Apply(query, new OozeModel { Filters = filter }));
        }

        [Theory]
        [InlineData("enabled", false)]
        [InlineData("-enabled", true)]
        public async Task Should_Correctly_Sort_Context_Data_By_Boolean(string sorter, bool inverted)
        {
            using var scope = _fixture.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            var oozeResolver = provider.GetRequiredService<IOozeResolver>();

            IQueryable<Post> query = context.Posts;
            query = oozeResolver.Apply(query, new OozeModel { Sorters = sorter });

            var results = await query.ToListAsync();
            var half = results.Count / 2;
            var firstHalf = results.Take(half)
                .ToList();
            var secondHalf = results.Skip(half)
                .ToList();

            firstHalf.ForEach(item => Assert.False(!inverted ? item.Enabled : !item.Enabled));
            secondHalf.ForEach(item => Assert.True(!inverted ? item.Enabled : !item.Enabled));
        }

        [Theory]
        [InlineData("id2", false)]
        [InlineData("-id2", true)]
        public async Task Should_Correctly_Sort_Context_Data_By_Id(string sorter, bool inverted)
        {
            using var scope = _fixture.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            var oozeResolver = provider.GetRequiredService<IOozeResolver>();

            IQueryable<Post> query = context.Posts;
            query = oozeResolver.Apply(query, new OozeModel { Sorters = sorter });

            var results = await query.ToListAsync();
            var half = results.Count / 2;
            var firstHalf = results.Take(half)
                .ToList();
            var secondHalf = results.Skip(half)
                .ToList();

            firstHalf.ForEach(item => Assert.True(!inverted ? item.Id <= 50 : item.Id > 50));
            secondHalf.ForEach(item => Assert.False(!inverted ? item.Id <= 50 : item.Id > 50));
        }

        [Theory]
        [InlineData("enabled", "id<=50")]
        public async Task Should_Correctly_Sort_And_Filter_Data(string sorter, string filter)
        {
            using var scope = _fixture.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            var oozeResolver = provider.GetRequiredService<IOozeResolver>();

            IQueryable<Post> query = context.Posts;
            query = oozeResolver.Apply(query, new OozeModel { Sorters = sorter, Filters = filter });

            var results = await query.ToListAsync();
            results.ForEach(item => Assert.True(item.Id <= 50));

            var groups = results.GroupBy(item => item.Enabled, (item, index) => index).ToList();
            Assert.True(groups.Count == 2);
        }
    }
}
