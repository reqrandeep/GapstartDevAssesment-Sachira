using Chinook.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Chinook.ClientModels;
using Microsoft.AspNetCore.Authorization;

namespace Chinook.Pages
{
    [Authorize]
    public class PlaylistPageModel : ComponentBase
    {
        [Inject]
        public IAlbumSevice PlaylistService { get; set; }
        [Parameter]
        public long PlaylistId { get; set; }
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }
        protected PlaylistDto CurrentPlaylist;
        protected string CurrentUserId;
        protected string InformationMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                CurrentUserId = await PlaylistService.GetUserId((await AuthenticationStateTask).User);
                await InvokeAsync(StateHasChanged);
                CurrentPlaylist = await PlaylistService.GetUserPlaylistById(PlaylistId, CurrentUserId);
            }
            catch (Exception ex)
            {
                InformationMessage = $"Error: An exception occurred while initializing: {ex.Message}";
            }
        }

        public async Task ToggleFavoriteStatus(PlaylistTrackDto track)
        {
            try
            {
                bool result = track.IsFavorite ? await PlaylistService.MarkAsUnFavourite(track.TrackId, CurrentUserId) : await PlaylistService.MarkAsFavourite(track.TrackId, CurrentUserId);
                InformationMessage = result ? $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} {(track.IsFavorite ? "removed from" : "added to")} playlist My favorite tracks." : $"Error: Failed to {(track.IsFavorite ? "remove" : "add")} track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} {(track.IsFavorite ? "from" : "to")} playlist My favorite tracks.";
                CurrentPlaylist = await PlaylistService.GetUserPlaylistById(PlaylistId, CurrentUserId);
            }
            catch (Exception ex)
            {
                InformationMessage = $"Error: An exception occurred while toggling favorite status: {ex.Message}";
            }
        }

        public async Task RemoveTrack(PlaylistTrackDto track)
        {
            try
            {
                bool result = await PlaylistService.RemoveTrackFromPlaylist(track.TrackId, PlaylistId, CurrentUserId);
                InformationMessage = result ? $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} has been successfully removed from playlist." : $"Error: Failed to remove track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} from playlist";
                CurrentPlaylist = await PlaylistService.GetUserPlaylistById(PlaylistId, CurrentUserId);
            }
            catch (Exception ex)
            {
                InformationMessage = $"Error: An exception occurred while removing track: {ex.Message}";
            }
        }

        public void CloseInformationMessage()
        {
            InformationMessage = "";
        }
    }
}
