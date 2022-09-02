namespace SOSync.Common.Services;

public interface IProgService
{
    Task<bool> TryAutoRegisterDbConfigAsync();
}
