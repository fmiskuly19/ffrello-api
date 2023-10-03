namespace FFrelloApi.Models
{
    public class NewBoardDto
    {
        public int WorkspaceId { get; set; }
        public string BoardTitle { get; set; } = String.Empty;
        public string Visibility { get; set; } = String.Empty;
    }
}
