﻿using Bogus;
using Microsoft.EntityFrameworkCore;

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

        if (await db.Set<Blog>().AnyAsync())
        {
            return;
        }

        var commentFaker = new Faker<Comment>()
            .RuleFor(comment => comment.Text, f => f.Lorem.Text())
            .RuleFor(comment => comment.UserId, f => f.Random.Uuid());

        var postFaker = new Faker<Post>()
            .RuleFor(post => post.Name, f => f.Name.FullName())
            .RuleFor(post => post.Body, f => f.Lorem.Paragraph())
            .RuleFor(post => post.Comment, f => commentFaker.Generate(60));

        var blogFaker = new Faker<Blog>()
            .RuleFor(blog => blog.Name, f => f.Name.FullName())
            .RuleFor(blog => blog.Posts, f => postFaker.Generate(100));


        var blogs = blogFaker.Generate(500);
        await db.Set<Blog>().AddRangeAsync(blogs);
        await db.SaveChangesAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
