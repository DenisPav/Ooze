using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ooze.Typed.Paging;
using Ooze.Typed.Web.Entities;

namespace Ooze.Typed.Web.Controllers;

[ApiController, Route("comments")]
public class CommentController : ControllerBase
{
    private readonly DatabaseContext _db;
    private readonly IOperationResolver<Comment, CommentFilters, CommentSorters> _resolver;

    public CommentController(
        DatabaseContext db,
        IOperationResolver<Comment, CommentFilters, CommentSorters> resolver)
    {
        _db = db;
        _resolver = resolver;
    }

    [HttpPost("typed")]
    public async Task<IActionResult> PostTyped(CommentsInput model)
    {
        IQueryable<Comment> query = _db.Set<Comment>();

        query = _resolver.Apply(query, model.Sorters, model.Filters, model.Paging);

        var results = await query.ToListAsync();
        return new ObjectResult(results);
    }
}

public record CommentsInput(CommentFilters Filters, IEnumerable<CommentSorters> Sorters, PagingOptions Paging);