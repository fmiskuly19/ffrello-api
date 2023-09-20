namespace test.Models
{
    public class Board
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsStarred { get; set; } = false;

        //navigation property
        public Workspace Workspace { get; set; }
        public int WorkspaceId { get; set; }

        public List<BoardList> BoardLists { get; set; }
    }
}
