using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class PersonalDetailModel 
    {
        public PersonalDetailModel()
        {
            FileModel = new FileModel();
        }

        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public int? FileId { get; set; }

        public FileModel FileModel { get; set; }

        

    }
}