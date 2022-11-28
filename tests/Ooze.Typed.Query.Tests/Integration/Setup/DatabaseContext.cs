using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.Query.Tests.Integration.Setup;

public class DatabaseContext : DbContext
{
    public const int TotalCountOfFakes = 100;
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

        var date = new DateTime(2022, 1, 1);
        var posts = Enumerable.Range(1, TotalCountOfFakes)
            .Select(id => new Post
            {
                Id = id,
                Enabled = id % 2 == 0,
                Name = $"{id}_Sample_post",
                Date = date.AddDays(id),
                Comments = new[] {
                    new Comment
                    {
                        Id = id,
                        Date = DateTime.Now.AddDays(id),
                        Text = $"Sample comment {id}",
                        User = new User
                        {
                            Id = id,
                            Email = $"sample_{id}@email.com"
                        }
                    }
                }
            });

        Posts.AddRange(posts);
        await SaveChangesAsync();
    }
}