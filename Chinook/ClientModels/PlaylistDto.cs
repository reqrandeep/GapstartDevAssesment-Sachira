namespace Chinook.ClientModels;

public class PlaylistDto
{
    public string Name { get; set; }
    public long PlaylistId { get; set; }
    public List<PlaylistTrackDto> Tracks { get; set; }
}