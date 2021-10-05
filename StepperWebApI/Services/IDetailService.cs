using Demo.Models;
using StepperWebApI;
using System.Linq;

namespace Demo.Service
{
    public interface IDetailService
    {
        IQueryable<Skill> GetAllSkill();

        Skill GetSkillById(int id);

        IQueryable<Detail> GetPersonalDetail();

        IQueryable<ExprienceDetail> GetOldExprienceDetail(int? detailId);

        IQueryable<EducationDetail> GetOldEducationDetail(int? detailId);

        TeamDetailModel GetTeamDetailById(int? id);

        bool AddTeamDetail(TeamDetailModel model);

        bool EditTeamDetail(TeamDetailModel model);

        bool DeleteTeamDetail(int id);

    }
}