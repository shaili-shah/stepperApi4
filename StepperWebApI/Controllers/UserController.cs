using Demo.Service;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace StepperWebApI.Controllers
{
   [RoutePrefix("User")]
    public class UserController : ApiController
    {
        private readonly IDetailService detailService;

        public UserController(IDetailService detailService)
        {
            this.detailService = detailService;
        }

        // GET: User
        [Route("GetDetails")]
        [HttpGet]
        public IEnumerable<string> GetDetails()
        {
            return new string[] { "test", "api" };
        }

        [Route("GetAllSkills")]
        [HttpGet]
        public List<Skill> GetAllSkills()
        {
            return detailService.GetAllSkill().ToList();
        }


    }
}