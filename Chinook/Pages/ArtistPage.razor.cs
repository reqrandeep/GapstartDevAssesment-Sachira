using Chinook.ClientModels;
using Chinook.Interfaces;
using Chinook.Shared.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Chinook.Pages
{
    [Authorize]
    public class ArtistPageModel : ComponentBase
    {
        [Inject]
        public IAlbumSevice ArtistService { get; set; }
        [Parameter]
        public long ArtistId { get; set; }
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }
        public ArtistDto Artist;
        public List<PlaylistTrackDto> Tracks;
        public string InfoMessage;
        public string CurrentUserId;
        public Modal PlaylistDialog { get; set; }
        public PlaylistTrackDto SelectedTrack;
        public List<PlaylistDto> Playlists;
        public bool IsNewPlaylistChecked = false;
        public long SelectedPlaylistId { get; set; }
        public string NewPlaylistName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await InvokeAsync(StateHasChanged);
                CurrentUserId = await ArtistService.GetUserId((await AuthenticationState).User);
                Artist = await ArtistService.GetArtistById(ArtistId);
                Tracks = await ArtistService.GetUserTracksByArtistId(ArtistId, CurrentUserId);
            }
            catch (Exception ex)
            {
                InfoMessage = $"Error: An exception occurred while initializing: {ex.Message}";
            }
        }

        public async Task ToggleFavoriteStatus(PlaylistTrackDto track)
        {
            try
            {
                CurrentUserId = await ArtistService.GetUserId((await AuthenticationState).User);
                bool result = false;
                if (track.IsFavorite)
                {
                    result =await ArtistService.MarkAsUnFavourite(track.TrackId, CurrentUserId);
                    InfoMessage = result ? $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist My favorite tracks." : $"Error: Failed to remove track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} from playlist My favorite tracks.";
                }
                else
                {
                    result = await ArtistService.MarkAsFavourite(track.TrackId, CurrentUserId);
                    InfoMessage = result ? $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist My favorite tracks." : $"Error: Failed to add track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} to playlist My favorite tracks.";
                }
                Tracks = await ArtistService.GetUserTracksByArtistId(ArtistId, CurrentUserId);
            }
            catch (Exception ex)
            {
                InfoMessage = $"Error: An exception occurred while updating favorite status: {ex.Message}";
            }
        }

        public async void AddTrackToPlaylist()
        {
            try
            {
                string playlistName;
                if (IsNewPlaylistChecked)
                {
                    await ArtistService.AddTrackToPlaylist(SelectedTrack.TrackId, null, CurrentUserId, NewPlaylistName);
                    InfoMessage = $"Track {SelectedTrack.ArtistName} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {NewPlaylistName}.";
                }
                else
                {
                    playlistName = Playlists.First(p => p.PlaylistId == SelectedPlaylistId).Name;
                    await ArtistService.AddTrackToPlaylist(SelectedTrack.TrackId, SelectedPlaylistId, CurrentUserId, playlistName);
                    InfoMessage = $"Track {SelectedTrack.ArtistName} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {playlistName}.";
                }
            }
            catch (Exception ex)
            {
                InfoMessage = $"Error: An exception occurred while adding the track to the playlist: {ex.Message}";
            }
            finally
            {
                PlaylistDialog.Close();
            }
        }

        public async void OpenPlaylistDialog(PlaylistTrackDto track)
        {
            try
            {
                SelectedTrack = track;
                CloseInfoMessage();
                Playlists = await ArtistService.GetAllPlaylistsExceptFavoriteByUserId(CurrentUserId);
                PlaylistDialog.Open();
            }
            catch (Exception ex)
            {
                InfoMessage = $"Error: An exception occurred while opening playlist dialog: {ex.Message}";
            }
        }

        public void TogglePlaylistOption()
        {
            IsNewPlaylistChecked = !IsNewPlaylistChecked;
        }

        public bool IsFormValid()
        {
            if (IsNewPlaylistChecked)
            {
                return !string.IsNullOrWhiteSpace(NewPlaylistName);
            }
            else
            {
                return SelectedPlaylistId != 0;
            }
        }

        public void CloseInfoMessage()
        {
            InfoMessage = "";
        }
    }
}
