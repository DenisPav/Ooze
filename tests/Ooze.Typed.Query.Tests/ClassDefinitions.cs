namespace Ooze.Typed.Query.Tests;

public record class Blog(int Id, string Name, int NumberOfComments, Guid ExternalId, DateTime Date);