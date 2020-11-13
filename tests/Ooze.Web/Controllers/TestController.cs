using Microsoft.AspNetCore.Mvc;
using Ooze.AspNetCore.Filters;
using System.Linq;

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
        public IActionResult Get([FromQuery] OozeModel model)
        {
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
