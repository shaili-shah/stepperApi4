using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StepperWebApI.Controllers
{
    [Route("User")]
    public class UserController : Controller
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