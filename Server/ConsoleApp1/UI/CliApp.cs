using RepositoryContracts;

namespace ConsoleApp1.UI;

public class CliApp
{
    private readonly UserView userView;
    private readonly PostView postView;
    private readonly CommentView commentView;

    public CliApp(IUserRepository userRepository, ICommentRepository commentRepository, IPostRepository postRepository)
    {
        userView = new UserView(userRepository);
        postView = new PostView(postRepository, userRepository, commentRepository);
        commentView = new CommentView(commentRepository, postRepository, userRepository);
    }

    public async Task StartAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine();
            Console.WriteLine("=== Main Menu ===");
            Console.WriteLine("1. Manage users");
            Console.WriteLine("2. Manage posts");
            Console.WriteLine("3. Manage comments");
            Console.WriteLine("0. Exit");

            int choice = ConsoleHelper.ReadInt("Choice: ");
            switch (choice)
            {
                case 1:
                    await userView.ShowMenuAsync();
                    break;
                case 2:
                    await postView.ShowMenuAsync();
                    break;
                case 3:
                    await commentView.ShowMenuAsync();
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
}
