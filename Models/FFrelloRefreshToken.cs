using test.Models;

namespace FFrelloApi.Models
{
    public class FFrelloRefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
