using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepositories
{
    private readonly List<User> users;

    public UserInMemoryRepositories()
    {
        users = new List<User>();
    }

  
}