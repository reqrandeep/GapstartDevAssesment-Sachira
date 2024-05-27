using Chinook.ClientModels;
using Chinook.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Chinook.Pages
{
    [Authorize]
    public class HomeModel : ComponentBase
    {
        [Inject]
        public IArtistService ArtistService { get; set; }
        public List<ArtistDto> ArtistList { get; set; }
        public string SearchQuery { get; set; }
        public Dictionary<long, long> AlbumCountDictionary { get; set; } = [];
        public string InfoMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await InvokeAsync(StateHasChanged);
                ArtistList = await ArtistService.GetAllArtists();
                AlbumCountDictionary = await ArtistService.GetAlbumCountForArtist();
            }
            catch (Exception ex)
            {
                InfoMessage = $"Error: An exception occurred while initializing: {ex.Message}";
            }
        }

        public async Task ExecuteSearch()
        {
            try
            {
                if (string.IsNullOrEmpty(SearchQuery))
                {
                    ArtistList = await ArtistService.GetAllArtists();
                }
                else
                {
                    ArtistList = await ArtistService.GetArtistsBySearchTerm(SearchQuery);
                }
            }
            catch (Exception ex)
            {
                InfoMessage = $"Error: An exception occurred while executing search: {ex.Message}";
            }
        }

        public async Task DisplayAllArtists()
        {
            try
            {
                ArtistList = await ArtistService.GetAllArtists();
            }
            catch (Exception ex)
            {
                InfoMessage = $"Error: An exception occurred while displaying all artists: {ex.Message}";
            }
        }

        public async Task OnSearchQueryChanged(ChangeEventArgs e)
        {
            try
            {
                SearchQuery = e.Value.ToString();
                if (string.IsNullOrEmpty(SearchQuery))
                {
                    await DisplayAllArtists();
                }
            }
            catch (Exception ex)
            {
                InfoMessage = $"Error: An exception occurred while changing search query: {ex.Message}";
            }
        }

        public void CloseInfoMessage()
        {
            InfoMessage = "";
        }
    }
}
