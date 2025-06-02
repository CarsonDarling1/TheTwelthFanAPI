
namespace TheTwelthFanAPI.Services;
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the given username.
    /// </summary>
    /// <param name="username">The username for which to generate the token.</param>
    /// <returns>A JWT token as a string.</returns>
    string GenerateToken(string username);
}
