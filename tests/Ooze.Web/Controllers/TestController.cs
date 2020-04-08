using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    Id = comment.Id
                }).ToList()
            };

            IQueryable<Post> query = _db.Posts
                .Include(post => post.Comments);

            query = _resolver.Apply(query, model);

            return Ok(query.ToList());
        }

        [HttpGet("query")]
        [ServiceFilter(typeof(OozeFilter<Post>))]
        public IQueryable<Post> GetQuery()
            => _db.Posts;
    }
}
