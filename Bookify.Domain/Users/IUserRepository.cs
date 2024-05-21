namespace Bookify.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetBuIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(User user);
}