using System;

namespace Demo.Models
{
    public class ExprienceDetailModel
    {
        public int? Id { get; set; }

        public string Company { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string CTC { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public int DetailId { get; set; }
    }
}