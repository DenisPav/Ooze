using Microsoft.AspNetCore.Mvc;
using Ooze.AspNetCore.Filters;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Ooze.Web.Controllers
{
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        readonly DatabaseContext _db;
        readonly IOozeResolver _resolver;

        public TestController(
            DatabaseContext db,
            IOozeResolver resolver)
        {
            _db = db;
            _resolver = resolver;
        }

        [HttpGet]
        public IActionResult Get([FromQuery]OozeModel model)
        {
            Expression<Func<Post, Post>> expr = post => new Post
            {
                Id = post.Id,
                Comments = post.Comments.Select(comment => new Comment
                {
                    Id = comment.Id,
                    User = new User
                    {
                        Id = comment.User.Id,
                        Email = comment.User.Email,
                        Comment = new Comment
                        {
                            Id = comment.User.Comment.Id,
                            User = new User
                            {
                                Id = comment.User.Comment.User.Id
                            }
                        }
                    }
                }).ToList()
            };

            IQueryable<Post> query = _db.Posts;

            query = _resolver.Apply(query, model);

            return Ok(query.ToList());
        }

        [HttpGet("query")]
        [ServiceFilter(typeof(OozeFilter<Post>))]
        public IQueryable<Post> GetQuery()
            => _db.Posts;
    }
}
