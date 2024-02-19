namespace FFrelloApi.Models
{
    public class CardChecklist
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;

        //nav props
        public Card Card { get; set; }
        public int CardId { get; set; }

        public List<CardChecklistItem> Items { get; set; } = new List<CardChecklistItem>();
    }

    public class CardChecklistItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public bool IsChecked { get; set; }

        //navigation props
        public CardChecklist? CardChecklist { get; set; }   
        public int CardChecklistId { get; set; }

        //user can be null, item might not have a user attached to it
        public User? User { get; set; }
        public int? UserId { get; set; }

        //duedate can be null, might not have a due date
        public DateTime? DueDate { get; set; }
    }
}
