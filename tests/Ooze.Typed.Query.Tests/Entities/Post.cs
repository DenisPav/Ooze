namespace Ooze.Typed.Query.Tests.Entities;

public class Post
{
    public long Id { get; set; }
    public Guid GuidId { get; set; }
    public string Name { get; set; }
    public bool Enabled { get; set; }
    public DateTime Date { get; set; }
    public DateOnly OnlyDate { get; set; }
    public decimal Rating { get; set; }
    public ICollection<Comment> Comments { get; set; }
}