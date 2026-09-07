using Entities;
using RepositoryContracts;

namespace ConsoleApp1.UI;

public class CommentView
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public CommentView(ICommentRepository commentRepository, IPostRepository postRepository, IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowMenuAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine();
            Console.WriteLine("--- Manage Comments ---");
            Console.WriteLine("1. Add comment to post");
            Console.WriteLine("2. Update existing comment");
            Console.WriteLine("3. Delete comment");
            Console.WriteLine("4. See all comments");
            Console.WriteLine("5. See comments by a specific user");
            Console.WriteLine("0. Back");

            int choice = ConsoleHelper.ReadInt("Choice: ");
            switch (choice)
            {
                case 1:
                    await CreateCommentAsync();
                    break;
                case 2:
                    await UpdateCommentAsync();
                    break;
                case 3:
                    await DeleteCommentAsync();
                    break;
                case 4:
                    ListAllComments();
                    break;
                case 5:
                    ListCommentsByUser();
                    break;
                case 0:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    private async Task CreateCommentAsync()
    {
        int postId = ConsoleHelper.ReadInt("Post id: ");
        if (!postRepository.GetMany().Any(p => p.Id == postId))
        {
            Console.WriteLine($"No post found with id {postId}.");
            return;
        }

        int userId = ConsoleHelper.ReadInt("User id: ");
        if (!userRepository.GetMany().Any(u => u.Id == userId))
        {
            Console.WriteLine($"No user found with id {userId}.");
            return;
        }

        string body = ConsoleHelper.ReadNonEmptyString("Comment: ");

        Comment comment = new Comment { Body = body, PostId = postId, UserId = userId };
        Comment created = await commentRepository.AddAsync(comment);
        Console.WriteLine($"Created comment with id {created.Id}.");
    }

    private async Task UpdateCommentAsync()
    {
        int id = ConsoleHelper.ReadInt("Id of comment to update: ");
        Comment existing;
        try
        {
            existing = await commentRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No comment found with id {id}.");
            return;
        }

        string body = ConsoleHelper.ReadNonEmptyString($"New text (was '{existing.Body}'): ");
        existing.Body = body;

        await commentRepository.UpdateAsync(existing);
        Console.WriteLine("Comment updated.");
    }

    private async Task DeleteCommentAsync()
    {
        int id = ConsoleHelper.ReadInt("Id of comment to delete: ");
        try
        {
            await commentRepository.DeleteAsync(id);
            Console.WriteLine("Comment deleted.");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No comment found with id {id}.");
        }
    }

    private void ListAllComments()
    {
        Console.WriteLine();
        foreach (Comment comment in commentRepository.GetMany())
        {
            Console.WriteLine($"[{comment.Id}] (post {comment.PostId}, user {comment.UserId}): {comment.Body}");
        }
    }

    private void ListCommentsByUser()
    {
        int userId = ConsoleHelper.ReadInt("User id: ");
        var comments = commentRepository.GetMany().Where(c => c.UserId == userId);

        foreach (Comment comment in comments)
        {
            Console.WriteLine($"[{comment.Id}] (post {comment.PostId}): {comment.Body}");
        }
    }
}
