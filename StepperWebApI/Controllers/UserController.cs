using Demo.Models;
using Demo.Service;
using System;
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

        #region get
        
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

        [Route("GetPersonalDetail")]
        [HttpGet]
        public List<Detail> GetPersonalDetail()
        {
            return detailService.GetPersonalDetail().ToList();
        }

        [Route("GetSkillById")]
        [HttpGet]
        public Skill GetSkillById(int id)
        {
            return detailService.GetSkillById(id);
        }

        [Route("GetTeamDetailById")]
        [HttpGet]
        public TeamDetailModel GetTeamDetailById(int id)
        {
            return detailService.GetTeamDetailById(id);
        }


        #endregion

        #region add/edit

        [Route("AddTeamDetail")]
        [HttpPost]
        public Tuple<object,bool> AddTeamDetail(TeamDetailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   return new Tuple<object, bool>("", detailService.AddTeamDetail(model));
                }
                catch(Exception ex)
                {
                    string msg = ex.InnerException.Message;
                    return new Tuple<object, bool>(msg,false);
                }

            }
            return new Tuple<object, bool>("", false);
        }

        [Route("EditTeamDetail")]
        [HttpPost]
        public Tuple<object,bool> EditTeamDetail(TeamDetailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return new Tuple<object, bool>("", detailService.EditTeamDetail(model));
                }
                catch (Exception ex)
                {
                    string msg = ex.InnerException.Message;
                    return new Tuple<object, bool>(msg, false);
                }

            }
            return new Tuple<object, bool>("", false);
        }

        #endregion

        #region delete 

        [Route("DeleteTeamDetail")]
        [HttpDelete]
        public Tuple<object,bool> DeleteTeamDetail(int id)
        {
            try
            {
                return new Tuple<object, bool>("", detailService.DeleteTeamDetail(id));
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException.Message;
                return new Tuple<object, bool>(msg,false);
            }
          
        }

        #endregion

    }
}