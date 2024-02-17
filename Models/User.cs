using FFrelloApi.Models;

namespace FFrelloApi.Models
{
    public class User
    {   
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string ProfilePhotoUrl { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public List<Workspace> Workspaces { get; set; } = new List<Workspace>();    

        // navigation properties
        public int RefreshTokenId { get; set; }
        public FFrelloRefreshToken RefreshToken { get; set; }  
    }
}
