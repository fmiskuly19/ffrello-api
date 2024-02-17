namespace FFrelloApi.Models
{
    public class CardComment
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }

        public Card Card { get; set; }
        public int CardId { get; set; }

        public string Value { get; set; } = String.Empty;

        public DateTime DateTime { get; set; }
    }
}
