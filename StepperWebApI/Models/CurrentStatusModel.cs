using System;

namespace Demo.Models
{
    public class CurrentStatusModel
    {
        public int CurrentStatusId { get; set; }

        public string Company { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string CTC { get; set; }

        public DateTime WorkingFrom { get; set; }

        public int DetailId { get; set; }
    }
}