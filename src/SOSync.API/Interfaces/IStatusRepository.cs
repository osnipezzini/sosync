using Refit;
using SOSync.Abstractions.Models;

namespace SOSync.API.Interfaces
{
    public interface IStatusRepository
    {
        [Get("/Status/list")]
        Task<List<Sync>?> GetStatusList();
    }
}
