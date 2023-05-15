namespace Ooze.Typed.Web.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public Guid UserId { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; }
}