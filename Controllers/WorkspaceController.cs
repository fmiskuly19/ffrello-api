using Microsoft.AspNetCore.Mvc;
using test.database;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        // GET: api/<WorkspaceController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<WorkspaceController>/5
        [HttpGet("{name}")]
        public ActionResult Get(string name)
        {
            using (TestContext dbContext = new TestContext())
            {
                var result = dbContext.Workspaces.Include(x => x.Boards).Single(x => x.Name == name);
                return Ok(result);
            }
        }

        // POST api/<WorkspaceController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<WorkspaceController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WorkspaceController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
