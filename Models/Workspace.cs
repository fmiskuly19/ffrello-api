namespace test.Models
{
    public class Workspace
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public string? Description { get; set; }    
        public string? Theme { get; set; }    

        public List<Board>? Boards { get; set; }
    }
}
