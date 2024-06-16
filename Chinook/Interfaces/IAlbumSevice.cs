using Chinook.ClientModels;
using System.Security.Claims;
using static Chinook.Services.AlbumService;

namespace Chinook.Interfaces
{
    public interface IAlbumSevice
    {
        Task<ArtistDto> GetArtistById(long artistId);
        Task<List<PlaylistTrackDto>> GetUserTracksByArtistId(long artistId, string userId);
        Task<string> GetUserId(ClaimsPrincipal user);
        Task<PlaylistDto> GetUserPlaylistById(long playlistId, string userId);
        Task<bool> MarkAsFavourite(long trackId, string currentUserId);
        Task<bool> MarkAsUnFavourite(long trackId, string currentUserId);
        Task<List<PlaylistDto>> GetAllPlaylistsExceptFavoriteByUserId(string userId);
        Task<bool> AddTrackToPlaylist(long trackId, long? playlistId, string currentUserId, string playlistName);
        Task<bool> RemoveTrackFromPlaylist(long trackId, long playlistId, string currentUserId);
        event NavMenuEventHandler SubscribeToNavMenu;
    }
}
