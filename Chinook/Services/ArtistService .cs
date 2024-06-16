using Chinook.ClientModels;
using Chinook.Context;
using Chinook.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        public delegate void NavMenuEventHandler(object sender, EventArgs e);
        public event NavMenuEventHandler SubscribeToNavMenu;

        public ArtistService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<ArtistDto>> GetAllArtists()
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var artists = dbContext.Artists;

            return await artists.Select(a => new ArtistDto
            {
                ArtistId = a.ArtistId,
                Name = a.Name,
                Albums = a.Albums.Select(al => new AlbumDto
                {
                    AlbumId = al.AlbumId,
                    Title = al.Title,
                    ArtistId = al.ArtistId
                }).ToList()
            }).ToListAsync();
        }

        public async Task<Dictionary<long, long>> GetAlbumCountForArtist()
        {
            var artists = await GetArtistsBySearchTerm();
            var artistAlbumCounts = new Dictionary<long, long>();

            foreach (var artist in artists)
            {
                using var dbContext = _dbFactory.CreateDbContext();
                artistAlbumCounts[artist.ArtistId] = await dbContext.Albums.CountAsync(a => a.ArtistId == artist.ArtistId);
            }
            return artistAlbumCounts;
        }

        public async Task<List<ArtistDto>> GetArtistsBySearchTerm(string searchTerm = null)
        {
            SubscribeToNavMenu?.Invoke(this, new EventArgs());
            using var dbContext = _dbFactory.CreateDbContext();
            var artists = dbContext.Artists.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                artists = artists.Where(a => a.Name.ToLower().Contains(searchTerm.ToLower()));
            }
            return await artists.Select(a => new ArtistDto
            {
                ArtistId = a.ArtistId,
                Name = a.Name,
                Albums = a.Albums.Select(al => new AlbumDto
                {
                    AlbumId = al.AlbumId,
                    Title = al.Title,
                    ArtistId = al.ArtistId
                }).ToList()
            }).ToListAsync();
        }
    }
}
