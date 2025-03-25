namespace Ooze.Typed.Query.Tests.Entities;

public class Post
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool Enabled { get; set; }
    public DateTime Date { get; set; }
    public DateOnly OnlyDate { get; set; }
    public ICollection<Comment> Comments { get; set; }
}