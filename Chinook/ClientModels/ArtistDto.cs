namespace Chinook.ClientModels
{
    public partial class ArtistDto
    {     
        public long ArtistId { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<AlbumDto> Albums { get; set; }
    }
}
