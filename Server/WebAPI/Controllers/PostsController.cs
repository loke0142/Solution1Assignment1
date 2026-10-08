using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepository;

    public PostsController(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto request)
    {
        Post post = new Post { Title = request.Title, Body = request.Body, UserId = request.UserId };
        Post created = await postRepository.AddAsync(post);
        PostDto dto = ToDto(created);
        return Created($"/Posts/{dto.Id}", dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePost(int id, [FromBody] UpdatePostDto request)
    {
        Post existing = await postRepository.GetSingleAsync(id);
        Post post = new Post { Id = id, Title = request.Title, Body = request.Body, UserId = existing.UserId };
        await postRepository.UpdateAsync(post);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost(int id)
    {
        await postRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetSinglePost(int id)
    {
        Post post = await postRepository.GetSingleAsync(id);
        return Ok(ToDto(post));
    }

    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetPosts([FromQuery] string? title, [FromQuery] int? userId)
    {
        IQueryable<Post> posts = postRepository.GetMany();

        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p => p.Title.Contains(title));
        }

        if (userId.HasValue)
        {
            posts = posts.Where(p => p.UserId == userId.Value);
        }

        List<PostDto> dtos = posts.Select(ToDto).ToList();
        return Ok(dtos);
    }

    private static PostDto ToDto(Post post)
    {
        return new PostDto { Id = post.Id, Title = post.Title, Body = post.Body, UserId = post.UserId };
    }
}
