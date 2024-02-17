namespace FFrelloApi.Models.Dto
{
    public class CardDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        public int BoardListId { get; set; }
        public string BoardListName { get; set; }
        public List<CardCommentDto> Comments { get; set; }
        public bool isUserWatching { get; set; }
    }
}
