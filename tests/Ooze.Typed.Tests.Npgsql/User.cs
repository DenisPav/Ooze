namespace Ooze.Typed.Tests.Npgsql;

public class User
{
    public long Id { get; set; }
    public string Email { get; set; }
    public Comment Comment { get; set; }
}