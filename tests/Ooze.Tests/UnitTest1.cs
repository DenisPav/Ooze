using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Configuration;
using System;
using Ooze.AspNetCore;
using Xunit;
using System.Linq;
using System.Threading.Tasks;

namespace Ooze.Tests
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

    public class UnitTest1 : IClassFixture<DbFixture<DatabaseContext>>
    {
        readonly DbFixture<DatabaseContext> _fixture;

        public UnitTest1(
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
        public async Task Test1(string filter, int expectedCount)
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
    }

    public class DbFixture<TContext>
        where TContext : DbContext
    {
        public DbFixture()
        {
            using var scope = CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            context.Prepare().Wait();
        }

        public IServiceCollection Services = new ServiceCollection()
            .AddDbContext<TContext>(opts => opts.UseSqlite("Data Source=./database.db;"))
            .AddOoze(typeof(UnitTest1).Assembly);

        IServiceProvider ServiceProvider => new DefaultServiceProviderFactory(
            new ServiceProviderOptions
            {
                ValidateScopes = true
            }).CreateServiceProvider(Services);

        public IServiceScope CreateScope() => ServiceProvider.CreateScope();
    }
}
