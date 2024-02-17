using Microsoft.AspNetCore.Mvc;
using FFrelloApi.database;
using Microsoft.EntityFrameworkCore;
using FFrelloApi.Models;
using FFrelloApi.Models.ApiArgs;
using Microsoft.AspNetCore.Authorization;
using FFrelloApi.Services;
using AutoMapper;

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
                    //check if user owns board List
                    //ensure user owns boardlist with boardListId
                    var card = await dbContext.Cards.Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == cardid);
                    var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

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
