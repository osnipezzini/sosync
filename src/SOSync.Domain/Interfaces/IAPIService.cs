using SOSync.Abstractions.Models;

namespace SOSync.Domain.Interfaces
{
    public interface IAPIService
    {
        Task<List<Sync>> GetSyncs();
    }
}
