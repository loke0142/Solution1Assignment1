using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> posts;

    public PostInMemoryRepository()
    {
        posts = new List<Post>();
        AddDummyData();
    }

    private void AddDummyData()
    {
        posts.Add(new Post { Id = 1, Title = "First post", Body = "This is the first dummy post.", UserId = 1 });
        posts.Add(new Post { Id = 2, Title = "Second post", Body = "This is the second dummy post.", UserId = 2 });
        posts.Add(new Post { Id = 3, Title = "Third post", Body = "This is the third dummy post.", UserId = 1 });
    }

    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any()
            ? posts.Max(p => p.Id) + 1
            : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }
    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{post.Id}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);

        return Task.CompletedTask;
    }
    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }
    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post == null)
        {
            throw new InvalidOperationException($"Post with id {id} not found");
        }
        return Task.FromResult(post);
    }
    public IQueryable<Post> GetMany()
    {
        return posts.AsQueryable();
    }
}