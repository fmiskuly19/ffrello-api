namespace FFrelloApi.Models.ApiArgs
{
    public class NewBoardDto
    {
        public int WorkspaceId { get; set; }
        public string BoardTitle { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
    }
}
