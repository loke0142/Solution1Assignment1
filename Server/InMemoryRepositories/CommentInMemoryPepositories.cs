using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository
{
    private readonly List<Comment> comments;

    public CommentInMemoryRepository()
    {
        comments = new List<Comment>();
    }

    
}