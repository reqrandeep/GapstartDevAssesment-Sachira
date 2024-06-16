using Chinook.ClientModels;
using Chinook.Context;
using Chinook.Interfaces;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Chinook.Services
{
    public class AlbumService : IAlbumSevice
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        public delegate void NavMenuEventHandler(object sender, EventArgs e);
        public event NavMenuEventHandler SubscribeToNavMenu;

        public AlbumService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<string> GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
        }

        public async Task<ArtistDto> GetArtistById(long artistId)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var result = await dbContext.Artists.SingleOrDefaultAsync(a => a.ArtistId == artistId);

            if (result == null)
            {
                return null;
            }

            return new ArtistDto
            {
                ArtistId = result.ArtistId,
                Name = result.Name
            };
        }

        public async Task<List<PlaylistDto>> GetAllPlaylistsExceptFavoriteByUserId(string userId)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            return await dbContext.Playlists
                .Where(p => p.UserPlaylists.Any(up => up.UserId == userId) && p.Name != "My favorite tracks")
                .Select(p => new PlaylistDto
                {
                    PlaylistId = p.PlaylistId,
                    Name = p.Name
                })
                .ToListAsync();
        }

        public async Task<PlaylistDto> GetUserPlaylistById(long playlistId, string userId)
        {
            using var dbContext = await _dbFactory.CreateDbContextAsync();
            var userPlaylist = dbContext.UserPlaylists
                .FirstOrDefault(up => up.PlaylistId == playlistId && up.UserId == userId);

            if (userPlaylist == null)
            {
                return null;
            }

            return await dbContext.Playlists
                .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
                .Where(p => p.PlaylistId == playlistId)
                .Select(p => new PlaylistDto
                {
                    Name = p.Name,
                    Tracks = p.Tracks.Select(t => new PlaylistTrackDto
                    {
                        AlbumTitle = t.Album.Title,
                        ArtistName = t.Album.Artist.Name,
                        TrackId = t.TrackId,
                        TrackName = t.Name,
                        IsFavorite = t.Playlists.Any(pl => pl.UserPlaylists.Any(up => up.UserId == userId && pl.Name == "My favorite tracks"))
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<List<PlaylistTrackDto>> GetUserTracksByArtistId(long artistId, string userId)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            return await dbContext.Tracks
                .Where(a => a.Album.ArtistId == artistId)
                .Include(a => a.Album)
                .Select(t => new PlaylistTrackDto
                {
                    AlbumTitle = t.Album == null ? "-" : t.Album.Title,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists
                        .Any(p => p.UserPlaylists
                            .Any(up => up.UserId == userId && up.Playlist.Name == "My favorite tracks"))
                }).ToListAsync();
        }

        public async Task<bool> MarkAsFavourite(long trackId, string currentUserId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var track = await dbContext.Tracks.FindAsync(trackId);
            var user = await dbContext.Users.FindAsync(currentUserId);

            var playlist = dbContext.Playlists
                .Include(p => p.UserPlaylists)
                .FirstOrDefault(p => p.Name == "My favorite tracks" && p.UserPlaylists.Any(up => up.UserId == currentUserId));

            if (playlist == null)
            {
                long newPlaylistId = dbContext.Playlists.Max(p => p.PlaylistId) + 1;
                playlist = new Playlist
                {
                    PlaylistId = newPlaylistId,
                    Name = "My favorite tracks",
                    Tracks = [],
                    UserPlaylists = [new UserPlaylist { UserId = currentUserId }]
                };
                dbContext.Playlists.Add(playlist);
                await dbContext.SaveChangesAsync();
            }

            playlist.Tracks.Add(track);
            await dbContext.SaveChangesAsync();

            SubscribeToNavMenu?.Invoke(this, new EventArgs());
            return true;
        }

        public async Task<bool> MarkAsUnFavourite(long trackId, string currentUserId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var track = await dbContext.Tracks.FindAsync(trackId);
            var user = await dbContext.Users.FindAsync(currentUserId);

            var playlist = dbContext.Playlists
                .Include(p => p.UserPlaylists)
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.Name == "My favorite tracks" && p.UserPlaylists.Any(up => up.UserId == currentUserId));

            playlist.Tracks.Remove(track);
            await dbContext.SaveChangesAsync();

            if (playlist.Tracks.Count == 0)
            {
                dbContext.Playlists.Remove(playlist);
                await dbContext.SaveChangesAsync();
            }

            SubscribeToNavMenu?.Invoke(this, new EventArgs());
            return true;
        }

        public async Task<bool> AddTrackToPlaylist(long trackId, long? playlistId, string currentUserId, string playlistName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var track = await dbContext.Tracks.FindAsync(trackId);
            var user = await dbContext.Users.FindAsync(currentUserId);

            var playlist = dbContext.Playlists
                .Include(p => p.UserPlaylists)
                .FirstOrDefault(p => p.Name == playlistName && p.UserPlaylists.Any(up => up.UserId == currentUserId));

            if (playlist == null)
            {
                long newPlaylistId = dbContext.Playlists.Max(p => p.PlaylistId) + 1;
                playlist = new Playlist
                {
                    PlaylistId = newPlaylistId,
                    Name = playlistName,
                    Tracks = [],
                    UserPlaylists = [new UserPlaylist { UserId = currentUserId }]
                };
                dbContext.Playlists.Add(playlist);
                await dbContext.SaveChangesAsync();
            }

            playlist.Tracks.Add(track);
            await dbContext.SaveChangesAsync();

            SubscribeToNavMenu?.Invoke(this, new EventArgs());
            return true;
        }

        public async Task<bool> RemoveTrackFromPlaylist(long trackId, long playlistId, string currentUserId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var playlist = dbContext.Playlists
                .Include(p => p.UserPlaylists)
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.PlaylistId == playlistId);

            var userPlaylist = playlist?.UserPlaylists.FirstOrDefault(up => up.UserId == currentUserId);
            if (playlist == null || userPlaylist == null)
            {
                return false;
            }

            var track = playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            if (track == null)
            {
                return false;
            }

            playlist.Tracks.Remove(track);
            await dbContext.SaveChangesAsync();

            SubscribeToNavMenu?.Invoke(this, new EventArgs());
            return true;
        }
    }
}
