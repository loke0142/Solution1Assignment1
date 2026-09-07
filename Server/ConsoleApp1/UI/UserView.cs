using Entities;
using RepositoryContracts;

namespace ConsoleApp1.UI;

public class UserView
{
    private readonly IUserRepository userRepository;

    public UserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowMenuAsync()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine();
            Console.WriteLine("--- Manage Users ---");
            Console.WriteLine("1. Create new user");
            Console.WriteLine("2. Update existing user");
            Console.WriteLine("3. Delete user");
            Console.WriteLine("4. See all users");
            Console.WriteLine("5. Search users by username");
            Console.WriteLine("0. Back");

            int choice = ConsoleHelper.ReadInt("Choice: ");
            switch (choice)
            {
                case 1:
                    await CreateUserAsync();
                    break;
                case 2:
                    await UpdateUserAsync();
                    break;
                case 3:
                    await DeleteUserAsync();
                    break;
                case 4:
                    ListAllUsers();
                    break;
                case 5:
                    SearchUsersByUsername();
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

    private async Task CreateUserAsync()
    {
        string userName = ConsoleHelper.ReadNonEmptyString("Username: ");

        if (userRepository.GetMany().Any(u => u.UserName == userName))
        {
            Console.WriteLine($"Username '{userName}' is already taken.");
            return;
        }

        string password = ConsoleHelper.ReadNonEmptyString("Password: ");

        User user = new User { UserName = userName, Password = password };
        User created = await userRepository.AddAsync(user);
        Console.WriteLine($"Created user with id {created.Id}.");
    }

    private async Task UpdateUserAsync()
    {
        int id = ConsoleHelper.ReadInt("Id of user to update: ");
        User existing;
        try
        {
            existing = await userRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No user found with id {id}.");
            return;
        }

        string userName = ConsoleHelper.ReadNonEmptyString($"New username (was '{existing.UserName}'): ");
        string password = ConsoleHelper.ReadNonEmptyString("New password: ");

        existing.UserName = userName;
        existing.Password = password;

        await userRepository.UpdateAsync(existing);
        Console.WriteLine("User updated.");
    }

    private async Task DeleteUserAsync()
    {
        int id = ConsoleHelper.ReadInt("Id of user to delete: ");
        try
        {
            await userRepository.DeleteAsync(id);
            Console.WriteLine("User deleted.");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No user found with id {id}.");
        }
    }

    private void ListAllUsers()
    {
        Console.WriteLine();
        foreach (User user in userRepository.GetMany())
        {
            Console.WriteLine($"[{user.Id}] {user.UserName}");
        }
    }

    private void SearchUsersByUsername()
    {
        string search = ConsoleHelper.ReadNonEmptyString("Search text: ");
        var matches = userRepository.GetMany()
            .Where(u => u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase));

        foreach (User user in matches)
        {
            Console.WriteLine($"[{user.Id}] {user.UserName}");
        }
    }
}
