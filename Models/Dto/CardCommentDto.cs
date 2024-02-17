namespace FFrelloApi.Models.Dto
{
    public class CardCommentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CardId { get; set; }

        public string Username { get; set; } = String.Empty;
        public string ProfilePhotoUrl { get; set; } = String.Empty;
        public string Value { get; set; } = String.Empty;
        public string Timestamp { get; set; } = String.Empty;
    }
}
