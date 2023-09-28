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
        public JsonResult GetWorkspaces(string userid)
        {
            try
            {
#if DEBUG
                Thread.Sleep(3000);
#endif

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

        [HttpPost("{userid}/newWorkspace")]
        public async Task<IActionResult> NewWorkspace(string userid, [FromBody] NewWorkspaceDto data)
        {
            try
            {
                using (FfrelloDbContext dbContext = new FfrelloDbContext())
                {
                    await dbContext.Workspaces.AddAsync(new Workspace() { Description = data.description, Name = data.workspaceName, Theme = data.theme });
                    await dbContext.SaveChangesAsync();
                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("dummy")]
        public JsonResult Dummy()
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

        [HttpGet("workspace/{id}")]
        public JsonResult Get(int id)
        {
            using (FfrelloDbContext dbContext = new FfrelloDbContext())
            {
                var result = dbContext.Workspaces.Include(x => x.Boards).Single(x => x.Id == id);
                return new JsonResult(result);
            }
        }

        //// POST api/<WorkspaceController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<WorkspaceController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<WorkspaceController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
