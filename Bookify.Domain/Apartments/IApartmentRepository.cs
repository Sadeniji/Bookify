namespace Bookify.Domain.Apartments;

public interface IApartmentRepository
{
    Task<Apartment?> GetBuIdAsync(Guid id, CancellationToken cancellationToken = default);

}