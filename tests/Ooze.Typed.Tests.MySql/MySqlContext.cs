using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Tests.Base;
using Ooze.Typed.Tests.Base.Setup;

namespace Ooze.Typed.Tests.MySql;

public class MySqlContext(DbContextOptions options) : GenericTestDbContext(options)
{
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
}