using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ooze.Configuration;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Ooze.Tests.Integration
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }


        public DatabaseContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var post = modelBuilder.Entity<Post>();
            post.HasKey(x => x.Id);
            post.Property(x => x.Id).ValueGeneratedOnAdd();
            post.HasMany(x => x.Comments)
                .WithOne();

            var comment = modelBuilder.Entity<Comment>();
            comment.HasKey(x => x.Id);
            comment.Property(x => x.Id).ValueGeneratedOnAdd();
            comment.HasOne(x => x.User)
                .WithOne(x => x.Comment)
                .HasForeignKey<Comment>(x => x.Id);

            var user = modelBuilder.Entity<User>();
            user.HasKey(x => x.Id);
            user.Property(x => x.Id);
        }

        public async Task Prepare()
        {
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();
            Posts.RemoveRange(Posts);

            var posts = Enumerable.Range(1, 100)
                    .Select(_ => new Post
                    {
                        Id = _,
                        Enabled = _ % 2 == 0,
                        Name = _.ToString(),
                        Comments = new[] {
                            new Comment
                            {
                                Id = _,
                                Date = DateTime.Now.AddDays(_),
                                Text = $"Sample comment {_}",
                                User = new User
                                {
                                    Id = _,
                                    Email = $"sample_{_}@email.com"
                                }
                            }
                        }
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
        public ICollection<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public User User { get; set; }
    }

    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public Comment Comment { get; set; }
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

        [Fact]
        public async Task Should_Correctly_Select_Simple_Data()
        {
            using var scope = _fixture.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            var oozeResolver = provider.GetRequiredService<IOozeResolver>();

            IQueryable<Post> query = context.Posts;
            query = oozeResolver.Apply(query, new OozeModel { Fields = "name" });

            var results = await query.ToListAsync();
            results.ForEach(item => Assert.True(item.Id == 0 && item.Enabled == false && item.Comments == null));
        }

        [Fact]
        public async Task Should_Correctly_Select_Complex_Data()
        {
            using var scope = _fixture.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            var oozeResolver = provider.GetRequiredService<IOozeResolver>();

            IQueryable<Post> query = context.Posts;
            query = oozeResolver.Apply(query, new OozeModel { Fields = "name,comments.user.email" });

            var results = await query.ToListAsync();
            results.ForEach(item => Assert.True(item.Id == 0 && item.Name != null
                && item.Comments != null
                && item.Comments.Any(comment => comment.User != null && comment.User.Email != null && comment.User.Id == 0)));
        }

        [Fact]
        public async Task Should_Correctly_Apply_Paging()
        {
            using var scope = _fixture.CreateScope(true);
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<DatabaseContext>();
            var oozeResolver = provider.GetRequiredService<IOozeResolver>();

            IQueryable<Post> query = context.Posts;
            var model = new OozeModel { Page = 2, PageSize = 33 };
            query = oozeResolver.Apply(query, model);

            var results = await query.ToListAsync();
            for (int i = 0; i < results.Count; i++)
            {
                var item = results[i];
                Assert.True(item.Id == (i + (model.PageSize * model.Page) + 1));
                Assert.True(item.Name == (i + (model.PageSize * model.Page) + 1).ToString());
            }

            Assert.True(results.Count == model.PageSize);
        }
    }
}
