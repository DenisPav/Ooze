using System.Data.Common;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Web.Entities;

namespace Ooze.Typed.Web;

public class SeedService(IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var sqliteDb = scope.ServiceProvider.GetRequiredService<SqliteDatabaseContext>();
        await SeedRecords(sqliteDb, cancellationToken);
    }

    private static async Task SeedRecords(
        DbContext db,
        CancellationToken cancellationToken)
    {
        await db.Database.EnsureCreatedAsync(cancellationToken);
        
        if (await db.Set<Blog>().AnyAsync(cancellationToken))
        {
            return;
        }

        var commentFaker = new Faker<Comment>()
            .RuleFor(comment => comment.Text, f => f.Lorem.Text())
            .RuleFor(comment => comment.UserId, f => f.Random.Uuid());

        var postFaker = new Faker<Post>()
            .RuleFor(post => post.Name, f => f.Name.FullName())
            .RuleFor(post => post.Body, f => f.Lorem.Paragraph())
            .RuleFor(post => post.Comment, f => commentFaker.Generate(20));

        var blogFaker = new Faker<Blog>()
            .RuleFor(blog => blog.Name, f => f.Name.FullName())
            .RuleFor(blog => blog.CreatedAt, f => f.Date.Past().ToUniversalTime())
            .RuleFor(blog => blog.Posts, f => postFaker.Generate(25));

        var blogs = blogFaker.Generate(100);
        await db.Set<Blog>().AddRangeAsync(blogs, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}