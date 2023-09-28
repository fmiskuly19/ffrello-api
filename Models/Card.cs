namespace test.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public BoardList BoardList { get; set; }
        public int BoardListId { get; set; }

        public List<User> Members { get; set; }
    }
}
