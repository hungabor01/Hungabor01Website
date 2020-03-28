namespace Hungabor01Website.Database.Core.Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Type { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public byte[] Data { get; set; }
    }
}
