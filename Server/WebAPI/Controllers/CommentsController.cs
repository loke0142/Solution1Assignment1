using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepository;

    public CommentsController(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto request)
    {
        Comment comment = new Comment { Body = request.Body, PostId = request.PostId, UserId = request.UserId };
        Comment created = await commentRepository.AddAsync(comment);
        CommentDto dto = ToDto(created);
        return Created($"/Comments/{dto.Id}", dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateComment(int id, [FromBody] UpdateCommentDto request)
    {
        Comment existing = await commentRepository.GetSingleAsync(id);
        Comment comment = new Comment { Id = id, Body = request.Body, PostId = existing.PostId, UserId = existing.UserId };
        await commentRepository.UpdateAsync(comment);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        await commentRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetSingleComment(int id)
    {
        Comment comment = await commentRepository.GetSingleAsync(id);
        return Ok(ToDto(comment));
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetComments([FromQuery] int? userId, [FromQuery] int? postId)
    {
        IQueryable<Comment> comments = commentRepository.GetMany();

        if (userId.HasValue)
        {
            comments = comments.Where(c => c.UserId == userId.Value);
        }

        if (postId.HasValue)
        {
            comments = comments.Where(c => c.PostId == postId.Value);
        }

        List<CommentDto> dtos = comments.Select(ToDto).ToList();
        return Ok(dtos);
    }

    private static CommentDto ToDto(Comment comment)
    {
        return new CommentDto { Id = comment.Id, Body = comment.Body, PostId = comment.PostId, UserId = comment.UserId };
    }
}
