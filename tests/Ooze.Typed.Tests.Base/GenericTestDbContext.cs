using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Tests.Base.Setup;

namespace Ooze.Typed.Tests.Base;

public abstract class GenericTestDbContext(DbContextOptions options)
    : DbContext(options)
{
    public const int TotalCountOfFakes = 100;

    public virtual async Task Seed()
    {
        var date = new DateTime(2022, 1, 1, 20, 20, 22);
        var posts = Enumerable.Range(1, TotalCountOfFakes)
            .Select(id => new Post
            {
                Id = 0,
                Enabled = id % 2 == 0,
                Name = $"{id}_Sample_post_{id}",
                Date = date.AddDays(id).ToUniversalTime(),
                Comments =
                [
                    new Comment
                    {
                        Id = 0,
                        Date = DateTime.Now.AddDays(id).ToUniversalTime(),
                        Text = $"Sample comment {id}",
                        User = new User
                        {
                            Id = 0,
                            Email = $"sample_{id}@email.com"
                        }
                    }
                ]
            });


        Set<Post>().AddRange(posts);
        await SaveChangesAsync();
    }
}