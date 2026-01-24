namespace Ooze.Typed.Tests.SqlServer.Setup;

public class User
{
    public long Id { get; set; }
    public string Email { get; set; }
    public Comment Comment { get; set; }
}