namespace Chinook.ClientModels
{
    public partial class AlbumDto
    {
        public long AlbumId { get; set; }
        public string Title { get; set; } = null!;
        public long ArtistId { get; set; }
    }
}
