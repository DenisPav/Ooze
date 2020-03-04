using Microsoft.AspNetCore.Mvc;
using Ooze.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ooze.Web.Controllers
{
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        readonly DatabaseContext _db;
        readonly IExpressionResolver _resolver;

        public TestController(
            DatabaseContext db,
            IExpressionResolver resolver)
        {
            _db = db;
            _resolver = resolver;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IQueryable<Post> query = _db.Posts;

            query = _resolver.Apply(query, null);

            return Ok(query.ToList());
        }
    }
}
