using Microsoft.AspNetCore.Mvc;
using test.database;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.Models;
using System.Runtime.InteropServices;
using FFrelloApi.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using FFrelloApi.Controllers;
using Microsoft.AspNetCore.Authentication;
using FFrelloApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace test.Controllers
{
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        private FFrelloAuthenticationService _authenticationService { get; set; }
        public WorkspaceController(FFrelloAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        #region boards

        //this is setup for auth
        [HttpPost("{userid}/board/new")]
        public async Task<IActionResult> NewBoard(string userid, [FromBody] NewBoardDto data)
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

        [HttpGet("{userid}/getBoard/{boardid}")]
        public async Task<IActionResult> GetBoardWithIncludes(string userid, int boardid)
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
                    return new JsonResult(new { boardReturn, workspaceReturn });
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{userid}/board/star/{boardid}")]
        public async Task<IActionResult> StarBoard(string userid, [FromBody] StarBoardDto data)
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
        [HttpGet("{userid}/workspaces")]
        public IActionResult GetWorkspaces(string userid)
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
        [HttpPost("{userid}/workspace/new")]
        public async Task<IActionResult> NewWorkspace(string userid, [FromBody] NewWorkspaceDto data)
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
        [HttpDelete("{userid}/workspace/remove/{workspaceid}")]
        public async Task<IActionResult> DeleteWorkspace(string userid, int workspaceid)
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

        [HttpPost("{userid}/card/new")]
        public async Task<IActionResult> NewCard(string userid, [FromBody] NewCardDto data)
        {
            try
            {
                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var c = new Card() { BoardListId = data.BoardListId, Title = data.Title };
                    await dbContext.Cards.AddAsync(c);
                    await dbContext.SaveChangesAsync();
                    return new JsonResult(c);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region board lists


        [HttpPut("{userid}/newBoardList")]
        public async Task<IActionResult> NewBoardList(string userid, [FromBody] NewBoardListDto data)
        {
            try
            {
                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
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

        [HttpDelete("{userid}/boardList/remove/{boardListId}")]
        public async Task<IActionResult> RemoveBoardList(string userid, int boardListId)
        {
            try
            {
                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var existingBoardList = dbContext.BoardLists.Single(x => x.Id == boardListId);
                    dbContext.BoardLists.Remove(existingBoardList);
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
