using System.Collections.Generic;
using System.Web.Http;

namespace StepperWebApI.Controllers
{
    [Route("User")]
    public class UserController : ApiController
    {
        // GET: User
        [Route("GetDetails")]
        [HttpGet]
        public IEnumerable<string> GetDetails()
        {
            return new string[] { "test", "api" };
        }
    }
}