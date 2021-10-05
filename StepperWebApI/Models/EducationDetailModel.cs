using System;

namespace Demo.Models
{
    public class EducationDetailModel
    {
        public int? Id { get; set; }

        public string Course { get; set; }

        public string University { get; set; }

        public DateTime PassedOn { get; set; }

        public string Grade { get; set; }

        public int DetailId { get; set; }
    }
}