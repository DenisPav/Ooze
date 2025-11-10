namespace Ooze.Typed.Tests.MySql.Setup;

public class Post
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool Enabled { get; set; }
    public DateTime Date { get; set; }
    public ICollection<Comment> Comments { get; set; }
}