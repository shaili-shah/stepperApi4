using AutoMapper;
using Demo.Models;
using Demo.Repository;
using StepperWebApI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Demo.Service
{
    public class DetailService : IDetailService
    {
        #region fields
       
        private IRepository<Skill> skillRepository;
        private IRepository<Detail> detailRepository;
        private IRepository<File> fileRepository;
        private IRepository<BankDetail> bankDetailRepository;
        private IRepository<ProfessionalDetail> professionalDetailRepository;
        private IRepository<CurrentStatu> currentStatusRepository;
        private IRepository<ExprienceDetail> exprienceDetailRepository;
        private IRepository<EducationDetail> educationDetailRepository;

        #endregion

        #region Constructor

        public DetailService(IRepository<Skill> skillRepository , 
            IRepository<Detail> detailRepository , IRepository<File> fileRepository ,
            IRepository<BankDetail> bankDetailRepository , IRepository<CurrentStatu> currentStatusRepository,
            IRepository<ExprienceDetail> exprienceDetailRepository , IRepository<EducationDetail> educationDetailRepository,
            IRepository<ProfessionalDetail> professionalDetailRepository)
        {
            this.skillRepository = skillRepository;
            this.detailRepository = detailRepository;
            this.fileRepository = fileRepository;
            this.bankDetailRepository = bankDetailRepository;
            this.currentStatusRepository = currentStatusRepository;
            this.exprienceDetailRepository = exprienceDetailRepository;
            this.educationDetailRepository = educationDetailRepository;
            this.professionalDetailRepository = professionalDetailRepository;
        }

        #endregion

        #region Get

        public IQueryable<Skill> GetAllSkill()
        {
            return skillRepository.Table;
        }

        public Skill GetSkillById(int id)
        {
            return skillRepository.GetById(id);
        }

        public IQueryable<Detail> GetPersonalDetail()
        {
            return detailRepository.Table.Include(x => x.CurrentStatus);
        }

        public IQueryable<ExprienceDetail> GetOldExprienceDetail(int? detailId)
        {
            return exprienceDetailRepository.Table.Where(x => x.DetailId == detailId);
        }

        public IQueryable<EducationDetail> GetOldEducationDetail(int? detailId)
        {
            return educationDetailRepository.Table.Where(x => x.DetailId == detailId);
        }

        public TeamDetailModel GetTeamDetailById(int? id)
        {
            TeamDetailModel model = new TeamDetailModel();
           
            var detail = detailRepository.Table.Include(x => x.BankDetails).Include(x => x.ProfessionalDetails)
               .Include(x => x.CurrentStatus).Include(x => x.ExprienceDetails).Include(x => x.EducationDetails)
               .FirstOrDefault(x => x.Id == id);

            if (detail != null)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Detail, TeamDetailModel>();
                    cfg.CreateMap<BankDetail, TeamDetailModel>();
                    cfg.CreateMap<ProfessionalDetail, TeamDetailModel>();
                    cfg.CreateMap<CurrentStatu, TeamDetailModel>();
                    cfg.CreateMap<ExprienceDetail, ExprienceDetailModel>();
                    cfg.CreateMap<EducationDetail, EducationDetailModel>();

                });
                IMapper mapper = config.CreateMapper();
                model = mapper.Map<Detail, TeamDetailModel>(detail);
                File file = fileRepository.GetById(detail.FileId);
                if (file != null)
                {
                    FileModel fileModel = new FileModel();
                    fileModel.Data = file.Data;
                    fileModel.ContentType = file.ContentType;
                    fileModel.Name = file.Name;
                    fileModel.Id = file.Id;
                    model.FileModel = fileModel;
                }

                if (detail.BankDetails.Any())
                {
                    model = mapper.Map(detail.BankDetails.FirstOrDefault(), model);
                    model.BankDetailId = detail.BankDetails.FirstOrDefault().Id;
                }
                if (detail.ProfessionalDetails.Any())
                {
                    model.ProfessionalDetailId = detail.ProfessionalDetails.FirstOrDefault().Id;
                    model.Year = detail.ProfessionalDetails.FirstOrDefault().Year;
                    model.Month = detail.ProfessionalDetails.FirstOrDefault().Month;
                    var skillIds = detail.ProfessionalDetails.FirstOrDefault().SkillIds;
                    if (!string.IsNullOrWhiteSpace(skillIds))
                    {
                        model.SkillIds = skillIds.Split(',').Select(int.Parse).ToList();
                    }
                    int? resumeId = detail.ProfessionalDetails.FirstOrDefault().FileId;
                    File resume = fileRepository.GetById(resumeId);
                    if (resume != null)
                    {
                        FileModel resumeFileModel = new FileModel();
                        resumeFileModel.Data = resume.Data;
                        resumeFileModel.ContentType = resume.ContentType;
                        resumeFileModel.Name = resume.Name;
                        resumeFileModel.Id = resume.Id;
                        model.ResumeFileModel = resumeFileModel;
                    }

                }
                if (detail.CurrentStatus.Any())
                {
                    model = mapper.Map(detail.CurrentStatus.FirstOrDefault(), model);
                    model.CurrentStatusId = detail.CurrentStatus.FirstOrDefault().Id;
                }
                if (detail.ExprienceDetails.Any())
                {
                    model.LstExprienceDetailModel = mapper.Map(detail.ExprienceDetails.ToList(), model.LstExprienceDetailModel);
                }
                if (detail.EducationDetails.Any())
                {
                    model.LstEducationDetailModel = mapper.Map(detail.EducationDetails.ToList(), model.LstEducationDetailModel);
                }

                model.Id = detail.Id;
            }

            return model;
        }

        #endregion

        #region Add

        public bool AddTeamDetail(TeamDetailModel model)
        {
            // mapping
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TeamDetailModel, PersonalDetailModel>();
                cfg.CreateMap<TeamDetailModel, BankDetailModel>();
                cfg.CreateMap<TeamDetailModel, ProfessionalDetailModel>();
                cfg.CreateMap<TeamDetailModel, CurrentStatusModel>();
                cfg.CreateMap<TeamModel, ExprienceDetailModel>();

            });

            // personal detail
            PersonalDetailModel personalDetailModel = new PersonalDetailModel();
            IMapper mapper = config.CreateMapper();
            personalDetailModel = mapper.Map<TeamDetailModel, PersonalDetailModel>(model);
            Detail detail = AddPersonalDetail(personalDetailModel);

            // bank detail
            BankDetailModel bankDetailModel = new BankDetailModel();
            bankDetailModel = mapper.Map<TeamDetailModel, BankDetailModel>(model);
            bankDetailModel.DetailId = detail.Id;
            AddBankDetail(bankDetailModel);

            // professional detail            
            ProfessionalDetailModel professionalDetailModel = new ProfessionalDetailModel();
            professionalDetailModel = mapper.Map<TeamDetailModel, ProfessionalDetailModel>(model);
            professionalDetailModel.DetailId = detail.Id;
            professionalDetailModel.SkillIds = model.SkillIds.Any() ? String.Join(",", model.SkillIds) : null;
            AddProfessionalDetail(professionalDetailModel);

            // current status
            CurrentStatusModel currentStatusModel = new CurrentStatusModel();
            currentStatusModel = mapper.Map<TeamDetailModel, CurrentStatusModel>(model);
            currentStatusModel.DetailId = detail.Id;
            AddCurrentStatus(currentStatusModel);

            // exprience detail            
            if (model.LstExprienceDetailModel != null && model.LstExprienceDetailModel.Any())
            {
                List<ExprienceDetailModel> lstExprienceDetailModel = new List<ExprienceDetailModel>();
                model.LstExprienceDetailModel.ToList().ForEach(s => s.DetailId = detail.Id);
                lstExprienceDetailModel = model.LstExprienceDetailModel;
                AddExpriencesDetail(lstExprienceDetailModel);
            }

            // education detail
            if (model.LstEducationDetailModel != null && model.LstEducationDetailModel.Any())
            {
                List<EducationDetailModel> lstEducationDetailModel = new List<EducationDetailModel>();
                model.LstEducationDetailModel.ToList().ForEach(s => s.DetailId = detail.Id);
                lstEducationDetailModel = model.LstEducationDetailModel;
                AddEducationDetail(lstEducationDetailModel);
            }

            return true;
        }

        public Detail AddPersonalDetail(PersonalDetailModel model)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PersonalDetailModel, Detail>();
                cfg.CreateMap<FileModel, File>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<PersonalDetailModel, Detail>(model);

            if (model.FileModel != null && model.FileModel.Data != null)
            {
                var file = mapper.Map<FileModel, File>(model.FileModel);
                fileRepository.Insert(file);
                
                data.FileId = file.Id;
            }

            detailRepository.Insert(data);
           
            return data;
        }

        public bool AddBankDetail(BankDetailModel model)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BankDetailModel, BankDetail>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<BankDetailModel, BankDetail>(model);
            bankDetailRepository.Insert(data);
           
            return true;
        }

        public bool AddProfessionalDetail(ProfessionalDetailModel model)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProfessionalDetailModel, ProfessionalDetail>();
                cfg.CreateMap<FileModel, File>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<ProfessionalDetailModel, ProfessionalDetail>(model);

            if (model.ResumeFileModel != null && model.ResumeFileModel.Data != null)
            {
                var file = mapper.Map<FileModel, File>(model.ResumeFileModel);
                fileRepository.Insert(file);
                data.FileId = file.Id;
            }
            professionalDetailRepository.Insert(data);
            return true;
        }

        public bool AddCurrentStatus(CurrentStatusModel model)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CurrentStatusModel, CurrentStatu>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<CurrentStatusModel, CurrentStatu>(model);

            currentStatusRepository.Insert(data);
            return true;
        }

        public bool AddExpriencesDetail(List<ExprienceDetailModel> lstmodel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ExprienceDetailModel, ExprienceDetail>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<List<ExprienceDetailModel>, List<ExprienceDetail>>(lstmodel);
            foreach(var item in data)
            {
                exprienceDetailRepository.Insert(item);
            }

            return true;
        }

        public bool AddEducationDetail(List<EducationDetailModel> lstmodel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EducationDetailModel, EducationDetail>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<List<EducationDetailModel>, List<EducationDetail>>(lstmodel);
            foreach (var item in data)
            {
                educationDetailRepository.Insert(item);
            }
            return true;
        }

        #endregion

        #region Edit

        public bool EditTeamDetail(TeamDetailModel model)
        {
            // mapping
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TeamDetailModel, PersonalDetailModel>();
                cfg.CreateMap<TeamDetailModel, BankDetailModel>();
                cfg.CreateMap<TeamDetailModel, ProfessionalDetailModel>();
                cfg.CreateMap<TeamDetailModel, CurrentStatusModel>();
                cfg.CreateMap<TeamModel, ExprienceDetailModel>();

            });

            // personal detail
            PersonalDetailModel personalDetailModel = new PersonalDetailModel();
            IMapper mapper = config.CreateMapper();
            personalDetailModel = mapper.Map<TeamDetailModel, PersonalDetailModel>(model);
            Detail detail = EditPersonalDetail(personalDetailModel);

            // bank detail
            BankDetailModel bankDetailModel = new BankDetailModel();
            bankDetailModel = mapper.Map<TeamDetailModel, BankDetailModel>(model);
            bankDetailModel.DetailId = detail.Id;
            EditBankDetail(bankDetailModel);

            // professional detail
            ProfessionalDetailModel professionalDetailModel = new ProfessionalDetailModel();
            professionalDetailModel = mapper.Map<TeamDetailModel, ProfessionalDetailModel>(model);
            professionalDetailModel.DetailId = detail.Id;
            professionalDetailModel.SkillIds = model.SkillIds.Any() ? String.Join(",", model.SkillIds) : null;
            EditProfessionalDetail(professionalDetailModel);

            // current status
            CurrentStatusModel currentStatusModel = new CurrentStatusModel();
            currentStatusModel = mapper.Map<TeamDetailModel, CurrentStatusModel>(model);
            currentStatusModel.DetailId = detail.Id;
            EditCurrentStatus(currentStatusModel);

            // exprience detail
            List<ExprienceDetailModel> lstExprienceDetailModel = new List<ExprienceDetailModel>();
            lstExprienceDetailModel = model.LstExprienceDetailModel;

            if (model.LstExprienceDetailModel != null && model.LstExprienceDetailModel.Any())
            {
                List<ExprienceDetail> oldExprienceDetails = new List<ExprienceDetail>();
                oldExprienceDetails = GetOldExprienceDetail(model.Id).ToList();
                if (oldExprienceDetails.Any())
                {
                    DeleteExprienceDetails(oldExprienceDetails);
                }
                lstExprienceDetailModel.ForEach(x => x.Id = 0);
                AddExpriencesDetail(lstExprienceDetailModel);

            }

            // education detail
            List<EducationDetailModel> lstEducationDetailModel = new List<EducationDetailModel>();
            lstEducationDetailModel = model.LstEducationDetailModel;

            if (model.LstEducationDetailModel != null && model.LstEducationDetailModel.Any())
            {
                List<EducationDetail> oldEducationDetails = new List<EducationDetail>();
                oldEducationDetails = GetOldEducationDetail(model.Id).ToList();
                if (oldEducationDetails.Any())
                {
                    DeleteEducationDetails(oldEducationDetails);
                }
                lstEducationDetailModel.ForEach(x => x.Id = 0);
                AddEducationDetail(lstEducationDetailModel);
            }

            return true;
        }

        public Detail EditPersonalDetail(PersonalDetailModel model)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PersonalDetailModel, Detail>();
                cfg.CreateMap<FileModel, File>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<PersonalDetailModel, Detail>(model);

            if (model.FileModel != null && model.FileModel.Data != null)
            {
                var file = mapper.Map<FileModel, File>(model.FileModel);
                fileRepository.Insert(file);

                data.FileId = file.Id;
            }
            detailRepository.Update(data);
          
            return data;
        }

        public bool EditBankDetail(BankDetailModel model)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BankDetailModel, BankDetail>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<BankDetailModel, BankDetail>(model);
            data.Id = model.BankDetailId;
            bankDetailRepository.Update(data);
            
            return true;
        }

        public bool EditProfessionalDetail(ProfessionalDetailModel model)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProfessionalDetailModel, ProfessionalDetail>();
                cfg.CreateMap<FileModel, File>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<ProfessionalDetailModel, ProfessionalDetail>(model);
            data.Id = model.ProfessionalDetailId;
            if (model.ResumeFileModel != null && model.ResumeFileModel.Data != null)
            {
                var file = mapper.Map<FileModel, File>(model.ResumeFileModel);
                fileRepository.Insert(file);               
                data.FileId = file.Id;
            }
            professionalDetailRepository.Update(data);
            
            return true;
        }

        public bool EditCurrentStatus(CurrentStatusModel model)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CurrentStatusModel, CurrentStatu>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<CurrentStatusModel, CurrentStatu>(model);
            data.Id = model.CurrentStatusId;
            currentStatusRepository.Update(data);
            
            return true;
        }


        #endregion

        #region Delete

        public bool DeleteExprienceDetails(List<ExprienceDetail> exprienceDetails)
        {
            foreach(var item in exprienceDetails)
            {
                exprienceDetailRepository.Delete(item);
            }           
            return true;
        }

        public bool DeleteEducationDetails(List<EducationDetail> educationDetails)
        {
            foreach (var item in educationDetails)
            {
                educationDetailRepository.Delete(item);
            }
            return true;
        }

        public bool DeleteTeamDetail(int id)
        {
            
            var detail = detailRepository.Table.Include(x=>x.BankDetails).Include(x=>x.ProfessionalDetails)
                .Include(x => x.CurrentStatus).Include(x => x.ExprienceDetails).Include(x => x.EducationDetails)
                .FirstOrDefault(x=>x.Id == id);

            if (detail != null)
            {
                BankDetail bankDetail = detail.BankDetails.FirstOrDefault();
                ProfessionalDetail professionalDetail = detail.ProfessionalDetails.FirstOrDefault();
                CurrentStatu currentStatu = detail.CurrentStatus.FirstOrDefault();
                List<ExprienceDetail> lstExprienceDetail = detail.ExprienceDetails.ToList();
                List<EducationDetail> lstEducationDetail = detail.EducationDetails.ToList();

                if (bankDetail != null)
                {
                    bankDetailRepository.Delete(bankDetail);
                }
                if (professionalDetail != null)
                {
                    if (professionalDetail.FileId != null)
                    {
                        File resumeFile = fileRepository.GetById(professionalDetail.FileId);
                        if (resumeFile != null) fileRepository.Delete(professionalDetail.File);
                    }
                    professionalDetailRepository.Delete(professionalDetail);
                }

                if (currentStatu != null)
                {
                    currentStatusRepository.Delete(currentStatu);
                }
                if (detail.ExprienceDetails.Any())
                {
                    DeleteExprienceDetails(lstExprienceDetail);
                }
                if (detail.EducationDetails.Any())
                {
                    DeleteEducationDetails(lstEducationDetail);
                }
                if (detail.FileId > 0)
                {
                    File file = fileRepository.GetById(detail.FileId);
                    if (file != null) fileRepository.Delete(detail.File);
                }
                detailRepository.Delete(detail);
            }
            return true;
        }

        #endregion
    }
}