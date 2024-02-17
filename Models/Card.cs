namespace FFrelloApi.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        //this is its parent
        public BoardList BoardList { get; set; }
        //parent id
        public int BoardListId { get; set; }
        public string BoardListName { get; set; }

        public List<User> Members { get; set; }
        public List<CardWatcher> CardWatchers { get; set; }
        public List<CardComment> Comments { get; set; }
    }
}
