using AutoMapper;
using Demo.Models;
using StepperWebApI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Demo.Repository
{
    public class DetailRepository : IDetail
    {
        private DemoEntities _context;

        public DetailRepository(DemoEntities objempcontext)
        {
            _context = objempcontext;
        }

        #region Get 

        public IEnumerable<Detail> GetPersonalDetail()
        {
            return _context.Details.Include(x => x.CurrentStatus).ToList();

        }

        public IEnumerable<Skill> GetAllSkills()
        {
            return _context.Skills.ToList();
        }

        public Skill GetSkillById(int id)
        {
            return _context.Skills.FirstOrDefault(x => x.Id == id);
        }

        public TeamDetailModel GetTeamDetailById(int? id)
        {
            TeamDetailModel model = new TeamDetailModel();
            var detail = _context.Details.Include(x => x.BankDetails).Include(x => x.ProfessionalDetails)
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
                File file = _context.Files.FirstOrDefault(x => x.Id == detail.FileId);
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
                    File resume = _context.Files.FirstOrDefault(x => x.Id == resumeId);
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


        public void InsertPersonalDetail(Detail detail)
        {
            _context.Details.Add(detail);
        }

        public void InsertBankDetail(BankDetail bankDetail)
        {
            _context.BankDetails.Add(bankDetail);
        }

        public void InsertProfessionalDetail(ProfessionalDetail professionalDetail)
        {
            _context.ProfessionalDetails.Add(professionalDetail);
        }

        public void InsertFile(File file)
        {
            _context.Files.Add(file);
        }

        public void InsertCurrentStatus(CurrentStatu currentStatus)
        {
            _context.CurrentStatus.Add(currentStatus);
        }

        public void InsertRangeExprienceDetail(List<ExprienceDetail> lstExprienceDetails)
        {
            _context.ExprienceDetails.AddRange(lstExprienceDetails);
        }

        public void InsertRangeEducationDetail(List<EducationDetail> lstEducationDetails)
        {
            _context.EducationDetails.AddRange(lstEducationDetails);
        }

        public void UpdatePersonalDetail(Detail detail)
        {
            _context.Entry(detail).State = EntityState.Modified;
        }

        public void UpdateBankDetail(BankDetail bankDetail)
        {
            _context.Entry(bankDetail).State = EntityState.Modified;
        }

        public void UpdateProfessionalDetail(ProfessionalDetail professionalDetail)
        {
            _context.Entry(professionalDetail).State = EntityState.Modified;
        }

        public void UpdateCurrentStatus(CurrentStatu currentStatu)
        {
            _context.Entry(currentStatu).State = EntityState.Modified;
        }

        public void UpdateExprienceDetail(ExprienceDetail exprienceDetail)
        {
            _context.Entry(exprienceDetail).State = EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }


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
            if (model.LstEducationDetailModel!= null && model.LstEducationDetailModel.Any())
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
                InsertFile(file);
                Save();

                data.FileId = file.Id;
            }

            InsertPersonalDetail(data);
            Save();

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


            InsertBankDetail(data);
            Save();

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
                InsertFile(file);
                Save();

                data.FileId = file.Id;
            }

            InsertProfessionalDetail(data);
            Save();

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

            InsertCurrentStatus(data);
            Save();

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

            InsertRangeExprienceDetail(data);
            Save();

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

            InsertRangeEducationDetail(data);
            Save();

            return true;
        }

        #region edit 

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

            if (model.LstExprienceDetailModel!= null && model.LstExprienceDetailModel.Any())
            {
                List<ExprienceDetail> oldExprienceDetails = new List<ExprienceDetail>();
                oldExprienceDetails = _context.ExprienceDetails.Where(x => x.DetailId == model.Id).ToList();
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
                oldEducationDetails = _context.EducationDetails.Where(x => x.DetailId == model.Id).ToList();
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
                InsertFile(file);
                Save();

                data.FileId = file.Id;
            }

            UpdatePersonalDetail(data);
            Save();

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
            UpdateBankDetail(data);
            Save();

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
                InsertFile(file);
                Save();

                data.FileId = file.Id;
            }

            UpdateProfessionalDetail(data);
            Save();
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
            UpdateCurrentStatus(data);
            Save();
            return true;
        }

        #endregion

        #region Delete

        public bool DeleteExprienceDetails(List<ExprienceDetail> exprienceDetails)
        {
            _context.ExprienceDetails.RemoveRange(exprienceDetails);
            Save();
            return true;
        }

        public bool DeleteEducationDetails(List<EducationDetail> educationDetails)
        {
            _context.EducationDetails.RemoveRange(educationDetails);
            Save();
            return true;
        }

        public bool DeleteBankDetail(BankDetail bankDetail)
        {
            _context.BankDetails.Remove(bankDetail);
            Save();
            return true;
        }

        public bool DeleteFile(File file)
        {
            _context.Files.Remove(file);
            Save();
            return true;
        }

        public bool DeleteProfessionalDetail(ProfessionalDetail professionalDetail)
        {
            _context.ProfessionalDetails.Remove(professionalDetail);
            Save();
            return true;
        }

        public bool DeleteCurrentStatus(CurrentStatu currentStatu)
        {
            _context.CurrentStatus.Remove(currentStatu);
            Save();
            return true;
        }

        public bool DeletePersonalDetail(Detail detail)
        {
            _context.Details.Remove(detail);
            Save();
            return true;
        }

        public bool DeleteTeamDetail(int id)
        {
            var detail = _context.Details.Include(x => x.BankDetails).Include(x => x.ProfessionalDetails)
                .Include(x => x.CurrentStatus).Include(x => x.ExprienceDetails).Include(x => x.EducationDetails)
                .FirstOrDefault(x => x.Id == id);

            if(detail != null)
            {
                BankDetail bankDetail = detail.BankDetails.FirstOrDefault();
                ProfessionalDetail professionalDetail = detail.ProfessionalDetails.FirstOrDefault();
                CurrentStatu currentStatu = detail.CurrentStatus.FirstOrDefault();
                List<ExprienceDetail> lstExprienceDetail = detail.ExprienceDetails.ToList();
                List<EducationDetail> lstEducationDetail = detail.EducationDetails.ToList();

                if(bankDetail != null)
                {
                    DeleteBankDetail(bankDetail);
                }
                if (professionalDetail != null)
                {
                    if (professionalDetail.FileId != null)
                    {
                        File resumeFile = _context.Files.FirstOrDefault(x => x.Id == professionalDetail.FileId);
                        if(resumeFile != null) DeleteFile(professionalDetail.File);
                    }
                    DeleteProfessionalDetail(professionalDetail);
                }

                if (currentStatu != null)
                {
                    DeleteCurrentStatus(currentStatu);
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
                    File file = _context.Files.FirstOrDefault(x => x.Id == detail.FileId);
                    if(file != null) DeleteFile(detail.File);
                }
                DeletePersonalDetail(detail);
            }
            return true;
        }


        #endregion

    }
}