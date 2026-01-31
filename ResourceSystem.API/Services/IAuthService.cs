namespace ResourceSystem.API.Services;

public interface IAuthService
{
    string GenerateToken(string username, string role);
}
