using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class TeamDetailModel
    {
        public TeamDetailModel()
        {
            SkillIds = new List<int>();
            LstExprienceDetailModel = new List<ExprienceDetailModel>();
            LstEducationDetailModel = new List<EducationDetailModel>();
        }
       
        public int? Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public int? FileId { get; set; }

        public FileModel FileModel { get; set; }

        // bank detail        

        public int? BankDetailId { get; set; }

        [Required]
        public string IFSC { get; set; }

        [Required]
        public string AccountNo { get; set; }
        
        [Required]
        public string PanCardNo { get; set; }
        
        [Required]
        public string AadharCardNo { get; set; }

        // professional detail

        public int? ProfessionalDetailId { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }

        public List<int> SkillIds { get; set; }

        public List<string> LstSkills { get; set; }

        public int? ResumeFileId { get; set; }

        public FileModel ResumeFileModel { get; set; }

        // current status
        
        public int? CurrentStatusId { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        public string Designation { get; set; }

        [Required]
        public string Department { get; set; }

        public string CTC { get; set; }

        [Required]
        public DateTime WorkingFrom { get; set; }

        // exprience detail
        public List<ExprienceDetailModel> LstExprienceDetailModel { get; set; }

        // education detail
        public List<EducationDetailModel> LstEducationDetailModel { get; set; }

    }
}