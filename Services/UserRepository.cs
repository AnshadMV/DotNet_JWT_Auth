public class UserRepository
{
    private readonly List<AppUser> _users = new();

    public AppUser GetByUsername(string username)
        => _users.FirstOrDefault(u => u.Username == username);

    public void AddUser(AppUser user)
    {
        _users.Add(user);
    }
    public List<AppUser> GetAllUsers()
    {
        return _users;
    }

}
