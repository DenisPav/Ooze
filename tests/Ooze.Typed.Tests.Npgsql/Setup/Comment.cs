namespace Ooze.Typed.Tests.Npgsql.Setup;

public class Comment
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public string Text { get; set; }
    public User User { get; set; }
}