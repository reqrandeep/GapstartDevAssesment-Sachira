# Changes Made

### 1. Data Retrieval Methods
Data retrieval methods were moved to separate classes using dependency injection. The following services were created:

builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IAlbumSevice, AlbumService>();
builder.Services.AddScoped<INavMenuService, NavMenuService>();

These services were then injected into the relevant page models. For example, in the HomeModel class, the IArtistService was injected as follows:

C#

public class HomeModel : ComponentBase
{
    [Inject]
    public IArtistService ArtistService { get; set; }
}

### 2.Favorite/Unfavorite Tracks
Functionality for favoriting and unfavoriting tracks was implemented. An automatic playlist named “My favorite tracks” is created when a track is favorited.

### 3.Search for Artist Name
A feature was added to allow users to search for artists by name.

### 4.User Playlists in Navbar
The user’s playlists are now listed in the left navbar. Any addition or modification to a playlist is reflected in the navbar without a full page reload.

### 5.Add Tracks to Playlist
Users can now add tracks to an existing or new playlist. The dialog for this functionality was already created but has now been completed.

### Authors
Sachira Randeep
Acknowledgments
Anyone you wish to acknowledge