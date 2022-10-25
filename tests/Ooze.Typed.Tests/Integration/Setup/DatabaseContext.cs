using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.Tests.Integration.Setup;

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
                Name = $"{_}_Sample_post",
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