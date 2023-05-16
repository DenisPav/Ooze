namespace Ooze.Typed.Web.Entities;

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? CreatedAt { get; set; }

    public ICollection<Post> Posts { get; set; }
}