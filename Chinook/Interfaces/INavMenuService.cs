using Chinook.ClientModels;
using System.Security.Claims;

namespace Chinook.Interfaces
{
    public interface INavMenuService
    {
        Task<List<PlaylistDto>> GetNavMenuPlaylists(string userId);
        Task<string> GetUserId(ClaimsPrincipal user);
    }
}
