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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace test.Controllers
{
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        #region boards


        [HttpPost("{userid}/board/new")]
        public async Task<IActionResult> NewBoard(string userid, [FromBody] NewBoardDto data)
        {
            try
            {
                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
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

        [HttpGet("{userid}/getBoardPage/{boardid}")]
        public async Task<IActionResult> GetBoardPage(string userid, int boardid)
        {
            try
            {
                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    Thread.Sleep(2000);

                    var board = await dbContext.Boards.Include(x => x.BoardLists).ThenInclude(x => x.Cards).SingleOrDefaultAsync(x => x.Id == boardid);

                    //how to get rid of these warnings?
                    var workspace = await dbContext.Workspaces.Include(x => x.Boards).SingleOrDefaultAsync(x => x.Boards.Any(x => x.WorkspaceId == board.WorkspaceId));
                    return new JsonResult(new { board, workspace });
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
        
        [HttpGet("{userid}/workspaces")]
        public IActionResult GetWorkspaces(string userid)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

                var userEmail = jsonToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;

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


        [HttpPost("{userid}/workspace/new")]
        public async Task<IActionResult> NewWorkspace(string userid, [FromBody] NewWorkspaceDto data)
        {
            try
            {
                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    await dbContext.Workspaces.AddAsync(new Workspace() { Description = data.description, Name = data.workspaceName, Theme = data.theme });
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


        [HttpDelete("{userid}/workspace/remove/{workspaceid}")]
        public async Task<IActionResult> DeleteWorkspace(string userid, int workspaceid)
        {
            try
            {
                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var existingWorkspace = dbContext.Workspaces.Single(x => x.Id == workspaceid);
                    dbContext.Workspaces.Remove(existingWorkspace);
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
