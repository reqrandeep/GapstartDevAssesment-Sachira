using Chinook.ClientModels;
using Chinook.Interfaces;
using Chinook.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Chinook.Shared
{
    public class NavMenuModel : ComponentBase, IDisposable
    {
        [Inject] 
        public INavMenuService NavMenuService { get; set; }
        [Inject] 
        public IAlbumSevice AlbumSevice { get; set; }
        [Inject] 
        public IArtistService ArtistService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [CascadingParameter] 
        private Task<AuthenticationState> AuthenticationState { get; set; }
        public string CurrentUserId;
        public bool collapseNavMenu = true;
        public List<PlaylistDto> playlists = [];
        public string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected override async Task OnInitializedAsync()
        {
            AlbumSevice.SubscribeToNavMenu += UpdatePlaylists;
            ArtistService.SubscribeToNavMenu += UpdatePlaylists;

            CurrentUserId = await NavMenuService.GetUserId((await AuthenticationState).User);
            playlists = await NavMenuService.GetNavMenuPlaylists(CurrentUserId);
        }

        public async void UpdatePlaylists(object sender, EventArgs e)
        {
            playlists = await NavMenuService.GetNavMenuPlaylists(CurrentUserId);
            StateHasChanged();
        }

        public void Dispose()
        {
            AlbumSevice.SubscribeToNavMenu -= UpdatePlaylists;
            ArtistService.SubscribeToNavMenu -= UpdatePlaylists;
        }

        public void NavigateToPlaylist(long playlistId)
        {
            NavigationManager.NavigateTo($"/playlist/{playlistId}", true);
        }

        public void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
