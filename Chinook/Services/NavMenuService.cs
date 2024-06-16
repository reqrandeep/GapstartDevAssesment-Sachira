using Chinook.Context;
using Chinook.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Chinook.ClientModels;

namespace Chinook.Services
{
    public class NavMenuService : INavMenuService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;

        public NavMenuService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<string> GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
        }

        public async Task<List<PlaylistDto>> GetNavMenuPlaylists(string userId)
        {
            using var DbContext = _dbFactory.CreateDbContext();
            return await DbContext.Playlists
                .Where(p => p.UserPlaylists.Any(up => up.UserId == userId) && p.Tracks.Any())
                .Select(p => new PlaylistDto
                {
                    PlaylistId = p.PlaylistId,
                    Name = p.Name,
                    Tracks = p.Tracks.Select(t => new PlaylistTrackDto()).ToList()
                })
                .OrderBy(p => p.Name != "My favorite tracks")
                .ThenBy(p => p.Name)
                .ToListAsync();
        }
    }
}
