using Bogus;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Web;
using Ooze.Typed.Web.Entities;

public class SeedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SeedService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var sqlServerDb = scope.ServiceProvider.GetRequiredService<SqlServerDatabaseContext>();

        await Task.WhenAll(SeedRecords(db, cancellationToken), SeedRecords(sqlServerDb, cancellationToken));
    }

    private static async Task SeedRecords(
        DbContext db,
        CancellationToken cancellationToken)
    {
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
            .RuleFor(blog => blog.CreatedAt, f => f.Date.Past())
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