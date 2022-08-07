using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ooze.Typed.Web.Controllers
{
    [ApiController, Route("~/")]
    public class BlogController : ControllerBase
    {
        private readonly DatabaseContext _db;
        private readonly IOozeTypedResolver<Blog, BlogFilters, BlogSorters> _resolver;

        public BlogController(
            DatabaseContext db,
            IOozeTypedResolver<Blog, BlogFilters, BlogSorters> resolver)
        {
            _db = db;
            _resolver = resolver;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] BlogFilters filters,
            [FromQuery] BlogSorters sorters)
        {
            IQueryable<Blog> query = _db.Set<Blog>();

            query = _resolver
                .WithQuery(query)
                .Sort(sorters)
                .Filter(filters)
                .Apply();

            var results = await query.ToListAsync();
            return Ok(results);
        }
    }
}
