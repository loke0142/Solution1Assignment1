using Entities;
using RepositoryContracts;

namespace ConsoleApp1.UI;

public class PostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;

    public PostView(IPostRepository postRepository, IUserRepository userRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
    }

    public async Task ShowMenuAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine();
            Console.WriteLine("--- Manage Posts ---");
            Console.WriteLine("1. Create new post");
            Console.WriteLine("2. Update existing post");
            Console.WriteLine("3. Delete post");
            Console.WriteLine("4. View posts overview");
            Console.WriteLine("5. View specific post");
            Console.WriteLine("6. View posts by user id");
            Console.WriteLine("0. Back");

            int choice = ConsoleHelper.ReadInt("Choice: ");
            switch (choice)
            {
                case 1:
                    await CreatePostAsync();
                    break;
                case 2:
                    await UpdatePostAsync();
                    break;
                case 3:
                    await DeletePostAsync();
                    break;
                case 4:
                    ViewPostsOverview();
                    break;
                case 5:
                    await ViewSinglePostAsync();
                    break;
                case 6:
                    ViewPostsByUserId();
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

    private async Task CreatePostAsync()
    {
        string title = ConsoleHelper.ReadNonEmptyString("Title: ");
        string body = ConsoleHelper.ReadNonEmptyString("Body: ");
        int userId = ConsoleHelper.ReadInt("User id: ");

        if (!userRepository.GetMany().Any(u => u.Id == userId))
        {
            Console.WriteLine($"No user found with id {userId}.");
            return;
        }

        Post post = new Post { Title = title, Body = body, UserId = userId };
        Post created = await postRepository.AddAsync(post);
        Console.WriteLine($"Created post with id {created.Id}.");
    }

    private async Task UpdatePostAsync()
    {
        int id = ConsoleHelper.ReadInt("Id of post to update: ");
        Post existing;
        try
        {
            existing = await postRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No post found with id {id}.");
            return;
        }

        string title = ConsoleHelper.ReadNonEmptyString($"New title (was '{existing.Title}'): ");
        string body = ConsoleHelper.ReadNonEmptyString("New body: ");

        existing.Title = title;
        existing.Body = body;

        await postRepository.UpdateAsync(existing);
        Console.WriteLine("Post updated.");
    }

    private async Task DeletePostAsync()
    {
        int id = ConsoleHelper.ReadInt("Id of post to delete: ");
        try
        {
            await postRepository.DeleteAsync(id);
            Console.WriteLine("Post deleted.");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No post found with id {id}.");
        }
    }

    private void ViewPostsOverview()
    {
        Console.WriteLine();
        foreach (Post post in postRepository.GetMany())
        {
            Console.WriteLine($"[{post.Title}, {post.Id}]");
        }
    }

    private async Task ViewSinglePostAsync()
    {
        int id = ConsoleHelper.ReadInt("Id of post to view: ");
        Post post;
        try
        {
            post = await postRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No post found with id {id}.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"Title: {post.Title}");
        Console.WriteLine($"Body: {post.Body}");
        Console.WriteLine("Comments:");

        var comments = commentRepository.GetMany().Where(c => c.PostId == post.Id);
        foreach (Comment comment in comments)
        {
            Console.WriteLine($"  - {comment.Body}");
        }
    }

    private void ViewPostsByUserId()
    {
        int userId = ConsoleHelper.ReadInt("User id: ");
        var posts = postRepository.GetMany().Where(p => p.UserId == userId);

        foreach (Post post in posts)
        {
            Console.WriteLine($"[{post.Title}, {post.Id}]");
        }
    }
}
