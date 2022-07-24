public class Post
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Body { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; }
    public ICollection<Comment> Comment { get; set; }
}
