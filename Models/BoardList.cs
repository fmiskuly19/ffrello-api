namespace test.Models
{
    public class BoardList
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Board Board { get; set; }
        public int BoardId { get; set; }

        public List<Card> Cards { get; set; }
    }
}
