using Microsoft.AspNetCore.Mvc;
using test.database;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FFrelloApi.Models;
using test.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace test.Controllers
{
    [Route("api/")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        [HttpGet("{userid}/workspaces")]
        public IActionResult GetWorkspaces(string userid)
        {
            try
            {
                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    var result = dbContext.Workspaces.Include(x => x.Boards).ToList();
                    return new JsonResult(result);
                }
            }
            catch(Exception e)
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

        [HttpPost("{userid}/board/new")]
        public async Task<IActionResult> NewBoard(string userid, [FromBody] NewBoardDto data)
        {
            try
            {
                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    await dbContext.Boards.AddAsync(new Board() { WorkspaceId = data.WorkspaceId, Name = data.BoardTitle  });
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

        //[HttpGet("{userid}/workspace/{workspaceid}")]
        //public async Task<IActionResult> GetWorkspace(string userid, int workspaceid)
        //{
        //    try
        //    {
        //        using (FfrelloDbContext dbContext = new FfrelloDbContext())
        //        {
        //            var workspace = await dbContext.Workspaces.Include(x => x.Boards).SingleOrDefaultAsync(x => x.Id == workspaceid);
        //            return new JsonResult(workspace);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

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
