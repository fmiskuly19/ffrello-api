namespace FFrelloApi.Models
{
    public class CardWatcher
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int CardId { get; set; }
        public Card Card { get; set; }
    }
}
