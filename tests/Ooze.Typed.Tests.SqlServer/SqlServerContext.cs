﻿using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.Tests.SqlServer;

public class SqlServerContext : DbContext
{
    public SqlServerContext(DbContextOptions options) : base(options)
    {}

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

    public async Task Seed()
    {
        var date = new DateTime(2022, 1, 1);
        var posts = Enumerable.Range(1, 100)
            .Select(id => new Post
            {
                Id = 0,
                Enabled = id % 2 == 0,
                Name = $"{id}_Sample_post",
                Date = date.AddDays(id),
                Comments = new[]
                {
                    new Comment
                    {
                        Id = 0,
                        Date = DateTime.Now.AddDays(id),
                        Text = $"Sample comment {id}",
                        User = new User
                        {
                            Id = 0,
                            Email = $"sample_{id}@email.com"
                        }
                    }
                }
            });

        
        Set<Post>().AddRange(posts);
        await SaveChangesAsync();
    }
}