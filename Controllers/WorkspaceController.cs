using Microsoft.AspNetCore.Mvc;
using test.database;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace test.Controllers
{
    [Route("api/")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        [HttpGet("workspaces")]
        public JsonResult Get()
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
