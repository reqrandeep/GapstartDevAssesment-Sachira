using Chinook.ClientModels;
using static Chinook.Services.ArtistService;

namespace Chinook.Interfaces
{
    public interface IArtistService
    {
        Task<List<ArtistDto>> GetAllArtists();      
        Task<List<ArtistDto>> GetArtistsBySearchTerm(string searchTerm = null);
        Task<Dictionary<long, long>> GetAlbumCountForArtist();
        event NavMenuEventHandler SubscribeToNavMenu;
    }
}
