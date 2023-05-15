using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ooze.Typed.Web.Entities;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired();
        builder.Property(x => x.Body)
            .IsRequired();

        builder.HasMany(x => x.Comment)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .IsRequired();
    }
}