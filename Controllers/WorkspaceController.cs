using Microsoft.AspNetCore.Mvc;
using FFrelloApi.database;
using Microsoft.EntityFrameworkCore;
using FFrelloApi.Models;
using FFrelloApi.Models.ApiArgs;
using Microsoft.AspNetCore.Authorization;
using FFrelloApi.Services;
using AutoMapper;
using FFrelloApi.Models.Dto;
using Microsoft.IdentityModel.Tokens;

namespace FFrelloApi.Controllers
{
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        private FFrelloAuthenticationService _authenticationService { get; set; }
        private IMapper _mapper { get; set; }
        public WorkspaceController(FFrelloAuthenticationService authenticationService, IMapper mapper)
        {
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        #region boards

        //this is setup for auth
        [HttpPost("board/new")]
        public async Task<IActionResult> NewBoard([FromBody] NewBoardDto data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    // Check if user matches the workspace id user
                    var userWithWorkspaces = await dbContext.Users
                        .Include(u => u.Workspaces) 
                        .FirstOrDefaultAsync(x => x.Email == userEmail);

                    if (userWithWorkspaces == null || !userWithWorkspaces.Workspaces.Any(w => w.Id == data.WorkspaceId))
                        return Unauthorized("User unauthorized");

                    await dbContext.Boards.AddAsync(new Board() { WorkspaceId = data.WorkspaceId, Name = data.BoardTitle });
                    await dbContext.SaveChangesAsync();

                    //return workspaces with boards for client to update
                    var updatedWorkspaces = dbContext.Workspaces.Include(x => x.Boards).ToList();
                    return new JsonResult(updatedWorkspaces);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //this is setup for auth
        [HttpGet("getBoard/{boardid}")]
        public async Task<IActionResult> GetBoardWithIncludes(int boardid)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    //ensure user owns the workspace that contains this board
                    var user = await dbContext.Users.FirstAsync(x => x.Email == userEmail);

                    //get board with matching id, include workspace and user to see if user matches
                    //first check if board exists
                    var board = await dbContext.Boards.Include(x => x.Workspace).ThenInclude(x => x.User).SingleOrDefaultAsync(x => x.Id == boardid);
                    if (board == null)
                        return BadRequest(String.Format("Could not find board with id {0}", boardid));

                    //check if board belongs to user
                    if (board.Workspace.User.Email != userEmail)
                        return Unauthorized();

                    //all good, get board with all its info
                    var boardReturn = await dbContext.Boards.Include(x => x.BoardLists).ThenInclude(x => x.Cards).SingleAsync(x => x.Id == boardid);
                    //get workspace
                    var workspaceReturn = await dbContext.Workspaces.Include(x => x.Boards).SingleOrDefaultAsync(x => x.Boards.Any(x => x.WorkspaceId == board.WorkspaceId));
                    return new JsonResult(new { board = boardReturn, workspace = workspaceReturn });
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("board/star/{boardid}")]
        public async Task<IActionResult> StarBoard([FromBody] StarBoardDto data)
        {
            try
            {
                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var board = dbContext.Boards.Single(x => x.Id == data.BoardId);
                    board.IsStarred = data.IsStarred;
                    dbContext.Boards.Attach(board);
                    await dbContext.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        #endregion

        #region workspaces
        
        //this is setup for auth
        [HttpGet("workspaces")]
        public IActionResult GetWorkspaces()
        {
            try
            {

                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var result = dbContext.Workspaces.Include(x => x.Boards).Include(x => x.User).Where(x => x.User.Email == userEmail).ToList();

                    return new JsonResult(result);
                }
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message);
            }
        }

        //this is setup for auth
        [HttpPost("workspace/new")]
        public async Task<IActionResult> NewWorkspace([FromBody] NewWorkspaceDto data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    //there is probably a faster way to do this
                    var user = await dbContext.Users.FirstAsync(x => x.Email == userEmail);
                    await dbContext.Workspaces.AddAsync(new Workspace() { Description = data.description, Name = data.workspaceName, Theme = data.theme, User = user });
                    await dbContext.SaveChangesAsync();

                    //return workspaces with boards for client to update
                    var updatedWorkspaces = dbContext.Workspaces.Include(x => x.Boards).ToList();

                    return new JsonResult(updatedWorkspaces);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //this is setup for auth
        [HttpDelete("workspace/remove/{workspaceid}")]
        public async Task<IActionResult> DeleteWorkspace(int workspaceid)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    //ensure user owns workspace before deleting
                    var user = await dbContext.Users.Include(x => x.Workspaces).FirstAsync(x => x.Email == userEmail);
                    if(!user.Workspaces.Any(x => x.Id == workspaceid))
                        return Unauthorized();

                    var existingWorkspace = user.Workspaces.Single(x => x.Id == workspaceid);
                    dbContext.Workspaces.Remove(existingWorkspace);
                    await dbContext.SaveChangesAsync();

                    //return workspaces with boards for client to update
                    var updatedWorkspaces = dbContext.Workspaces.Include(x => x.Boards).Where(x => x.User.Id == user.Id).ToList();
                    return new JsonResult(updatedWorkspaces);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region board lists

        //this is setup for auth
        [HttpPut("newBoardList")]
        public async Task<IActionResult> NewBoardList([FromBody] NewBoardListDto data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    //ensure user owns board with boardid
                    var board = await dbContext.Boards.Include(x => x.Workspace).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == data.BoardId);

                    //check if board exists
                    if (board == null)
                        return BadRequest(String.Format("Could not find board with id {0}", data.BoardId));
                    //check if board list belongs to user
                    if (board.Workspace.User.Email != userEmail)
                        return Unauthorized();

                    var bl = new BoardList() { Name = data.Name, BoardId = data.BoardId };
                    await dbContext.BoardLists.AddAsync(bl);
                    await dbContext.SaveChangesAsync();
                    return new JsonResult(bl);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //this is setup for auth
        [HttpDelete("boardList/remove/{boardListId}")]
        public async Task<IActionResult> RemoveBoardList(int boardListId)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    //check if user owns board List
                    //ensure user owns boardlist with boardListId
                    var boardList = await dbContext.BoardLists.Include(x => x.Board).ThenInclude(x => x.Workspace).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == boardListId);

                    //check if board list exists
                    if (boardList == null)
                        return BadRequest(String.Format("Could not find board list with id {0}", boardListId));
                    //check if board belongs to user
                    if (boardList.Board.Workspace.User.Email != userEmail)
                        return Unauthorized();

                    dbContext.BoardLists.Remove(boardList);
                    await dbContext.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region cards

        [HttpPost("card/new")]
        public async Task<IActionResult> NewCard([FromBody] NewCardDto data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    //check if user owns board List
                    //ensure user owns boardlist with boardListId
                    var boardList = await dbContext.BoardLists.Include(x => x.Board).ThenInclude(x => x.Workspace).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == data.BoardListId);

                    //check if board list exists
                    if (boardList == null)
                        return BadRequest(String.Format("Could not find board list with id {0}", data.BoardListId));
                    //check if board list belongs to user
                    if (boardList.Board.Workspace.User.Email != userEmail)
                        return Unauthorized();

                    //all good, create new card for this boardlist and save
                    var c = new Card() { BoardListId = data.BoardListId, Title = data.Title };
                    await dbContext.Cards.AddAsync(c);
                    await dbContext.SaveChangesAsync();

                    //return the new card with newly created id
                    return new JsonResult(c);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("card/{cardid}")]
        public async Task<IActionResult> GetCard(int cardid)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var card = await dbContext.Cards
                        .Include(x => x.Members)
                        .Include(x => x.Comments).ThenInclude(x => x.User)
                        .Include(x => x.Checklists).ThenInclude(x => x.Items)
                        .FirstOrDefaultAsync(x => x.Id == cardid);

                    var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                    if (card == null || user == null)
                    {
                        return BadRequest("ERROR GETTING CARD OR USER");
                    }
                    //reorder card comments so that the most recent shows up on top
                    //there is definitely a better way to do this, maybe through AutoMapper?????
                    card.Comments = card.Comments.OrderByDescending(x => x.DateTime).ToList();

                    var watcher = await dbContext.CardWatchers.FirstOrDefaultAsync(x => x.CardId == card.Id && x.UserId == user.Id);
                    var response = _mapper.Map<CardDto>(card);
                    response.isUserWatching = watcher == null ? false : true;

                    return new JsonResult(response);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public class WatchDto
        {
            public bool Watch { get; set; }
            public int UserId { get; set; }
            public int CardId { get; set; }
        }

        [HttpPut("card/{cardid}/watch")]
        public async Task<IActionResult> WatchCard(int cardid, [FromBody] WatchDto data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var card = await dbContext.Cards.Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == data.CardId);
                    var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                    if(card == null || user == null)
                    {
                        return BadRequest(String.Format("ERROR {0}", data.CardId));
                    }

                    var response = _mapper.Map<CardDto>(card);

                    //add new card watcher
                    if (data.Watch)
                    {
                        //check if exists before adding
                        var watcher = await dbContext.CardWatchers.FirstOrDefaultAsync(x => x.UserId == user.Id && x.CardId == card.Id);
                        if (watcher == null)
                        {
                            dbContext.CardWatchers.Add(new CardWatcher() { UserId = user.Id, CardId = card.Id });
                            await dbContext.SaveChangesAsync();
                        }
                        response.isUserWatching = true;
                        return new JsonResult(response);
                    }
                    //delete existing card watcher if it exists
                    else
                    {
                        var watcher = await dbContext.CardWatchers.FirstOrDefaultAsync(x => x.UserId == user.Id && x.CardId == card.Id);
                        if (watcher != null)
                        {
                            dbContext.CardWatchers.Remove(watcher);
                            await dbContext.SaveChangesAsync();

                            response.isUserWatching = false;
                            return new JsonResult(response);
                        }

                        return BadRequest(String.Format("Could not find watcher for user {0} and card {1}",user.Id, card.Id));
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region card extras

        public class NewChecklistArgs
        {
            public int CardId { get; set; }
            //must have a name
            public string Name { get; set; }
        }

        //create new checklist
        [HttpPost("card/checklist/new")]
        public async Task<IActionResult> NewChecklist([FromBody] NewChecklistArgs data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                if (String.IsNullOrEmpty(data.Name))
                {
                    return BadRequest("Checklits must have a name");
                }

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var newChecklist = new CardChecklist
                    {
                        Name = data.Name,
                        CardId = data.CardId
                    };

                    dbContext.CardChecklists.Add(newChecklist);
                    await dbContext.SaveChangesAsync();

                    //return the new card with newly created id
                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public class NewChecklistItemArgs
        {
            public int ChecklistId { get; set; }
            public string Name { get; set; } = String.Empty;
            public bool IsChecked { get; set; } = false;
            public int? UserId { get; set; }
            public DateTime? DueDate { get; set; }
        }

        //create new checklist item
        [HttpPost("card/checklist/item/add")]
        public async Task<IActionResult> NewChecklistItem([FromBody] NewChecklistItemArgs data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                if (String.IsNullOrEmpty(data.Name))
                {
                    return BadRequest("Checklist item must have a value");
                }

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var checklist = await dbContext.CardChecklists.FirstOrDefaultAsync(x => x.Id == data.ChecklistId);

                    if (checklist == null)
                        return BadRequest("Could not find checklist");

                    checklist.Items.Add(new CardChecklistItem() { Name = data.Name, DueDate = data.DueDate, UserId = data.UserId, CardChecklistId = data.ChecklistId });
                    dbContext.Attach(checklist);
                    await dbContext.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public class SetChecklistItemValueArgs
        {
            public int ChecklistItemId { get; set; }
            public bool Value { get; set; }

        }

        [HttpPost("card/checklist/item/edit")]
        public async Task<IActionResult> SetChecklistItemValue([FromBody] SetChecklistItemValueArgs data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var checklistItem = await dbContext.CardChecklistItems.FirstOrDefaultAsync(x => x.Id == data.ChecklistItemId);

                    if (checklistItem == null)
                        return BadRequest("Could not find checklist item");

                    checklistItem.IsChecked = data.Value;
                    dbContext.Attach(checklistItem);
                    await dbContext.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //edit checklist

        //delete checklist
        public class RemoveChecklistArgs
        {
            public int ChecklistId { get; set; }
        }

        [HttpDelete("card/checklist/remove")]
        public async Task<IActionResult> DeleteChecklist([FromBody] RemoveChecklistArgs data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var checklist = await dbContext.CardChecklists.FirstOrDefaultAsync(x => x.Id == data.ChecklistId);

                    if (checklist == null)
                        return BadRequest("Could not find checklist");

                    dbContext.CardChecklists.Remove(checklist);
                    await dbContext.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region comments

        public class AddCommentArgs
        {
            public int UserId { get; set; }
            public int CardId { get; set; }
            public string Comment { get; set; } = String.Empty;
        }

        [HttpPost("card/comment/new")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentArgs data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var newComment = new CardComment()
                    {
                        UserId = data.UserId,
                        CardId = data.CardId,
                        Value = data.Comment,
                        DateTime = DateTime.UtcNow
                    };
                    dbContext.CardComments.Add(newComment);
                    await dbContext.SaveChangesAsync();

                    var comments = dbContext.CardComments.Where(x => x.CardId == data.CardId).Include(x => x.User).ToList().OrderByDescending(x => x.DateTime);
                    var commentDtos = _mapper.Map<List<CardCommentDto>>(comments);
                    
                    //this is probably bad for performance
                    //return comments
                    return new JsonResult(commentDtos);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public class RemoveCommentArgs
        {
            public int CommentId { get; set; }
        }

        [HttpDelete("card/comment/remove")]
        public async Task<IActionResult> RemoveComment([FromBody] RemoveCommentArgs data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var comment = await dbContext.CardComments.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == data.CommentId);

                    if(comment == null)
                        return BadRequest("Could not find comment");
                    else if (comment.User.Email != userEmail)
                        return BadRequest("Cannot delete a comment not owned by the user");

                    dbContext.CardComments.Remove(comment);
                    await dbContext.SaveChangesAsync();
                    
                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public class EditCommentArgs
        {
            public int CommentId { get; set; }
            public string Value { get; set; }
        }

        [HttpPut("card/comment/edit")]
        public async Task<IActionResult> EditComment([FromBody] EditCommentArgs data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var comment = await dbContext.CardComments.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == data.CommentId);

                    if (comment == null)
                        return BadRequest("Could not find comment");
                    else if (comment.User.Email != userEmail)
                        return BadRequest("Cannot edit a comment not owned by the user");

                    comment.Value = data.Value;
                    await dbContext.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public class EditDescriptionArgs
        {
            public int CardId { get; set; }
            public string Value { get; set; }
        }

        [HttpPut("card/description/edit")]
        public async Task<IActionResult> EditDescription([FromBody] EditDescriptionArgs data)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!_authenticationService.IsJwtValid(jwtToken, out string error, out string userEmail))
                    return Unauthorized(error);

                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var card = await dbContext.Cards.FirstOrDefaultAsync(x => x.Id == data.CardId);

                    if (card == null)
                        return BadRequest("Could not find card");
                    else if (String.IsNullOrEmpty(data.Value))
                        return BadRequest("Description must have a value");

                    card.Description = data.Value;
                    await dbContext.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        [HttpGet("dummy")]
        public IActionResult Dummy()
        {
            try
            {
                Thread.Sleep(3000);

                return new JsonResult("ok");
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message);
            }
        }
    }
}
