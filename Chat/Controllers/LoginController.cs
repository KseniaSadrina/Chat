using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [AllowAnonymous]
  public class LoginController : ControllerBase
  {

      // GET: api/Login/5
      [HttpGet("{id}", Name = "Get")]
      public string Get(int id)
      {
          return "value";
      }

      // POST: api/Login
      [HttpPost]
      public void Post([FromBody] string value)
      {
      }

      // PUT: api/Login/5
      [HttpPut("{id}")]
      public void Put(int id, [FromBody] string value)
      {
      }

      // DELETE: api/ApiWithActions/5
      [HttpDelete("{id}")]
      public void Delete(int id)
      {
      }
  }
}
