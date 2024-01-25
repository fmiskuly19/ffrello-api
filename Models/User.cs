using FFrelloApi.Models;

namespace test.Models
{
    public class User
    {   
        public int Id { get; set; }
        public string Email { get; set; }
        public List<Workspace> Workspaces { get; set; }

        // navigation properties
        public int RefreshTokenId { get; set; }
        public FFrelloRefreshToken RefreshToken { get; set; }  
        
    }
}
