using Demo.Models;
using StepperWebApI;
using System.Collections.Generic;

namespace Demo.Repository
{
    interface IDetail
    {
        IEnumerable<Detail> GetPersonalDetail();

        IEnumerable<Skill> GetAllSkills();

        bool AddTeamDetail(TeamDetailModel model);

        Detail AddPersonalDetail(PersonalDetailModel model);

        bool AddBankDetail(BankDetailModel model);

        bool AddProfessionalDetail(ProfessionalDetailModel model);

        bool AddCurrentStatus(CurrentStatusModel model);

        bool AddExpriencesDetail(List<ExprienceDetailModel> lstmodel);

        bool AddEducationDetail(List<EducationDetailModel> lstmodel);

        TeamDetailModel GetTeamDetailById(int? id);

        bool EditTeamDetail(TeamDetailModel model);

        bool DeleteTeamDetail(int id);

        Skill GetSkillById(int id);
    }
}