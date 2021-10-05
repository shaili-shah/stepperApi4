namespace Demo.Models
{
    public class ProfessionalDetailModel
    {
        public ProfessionalDetailModel()
        {
            ResumeFileModel = new FileModel();
        }

        public int ProfessionalDetailId { get; set; }

        public int? Year { get; set; }

        public int? Month { get; set; }

        public string SkillIds { get; set; }

        public int DetailId { get; set; }

        public int?  FileId { get; set; }

        public FileModel ResumeFileModel { get; set; }
    }
}