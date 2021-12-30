using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

//Name: Ba Binh Luong,
//Done for: Project 2 Part A, B, and C
//Last Modified: 2020 October 09 3:00pm

namespace Bluong4_Job_Bank.Models
{
    public class RetrainingProgram
    {
        public RetrainingProgram()
        {
            this.Applicants = new HashSet<Applicant>();
        }
        public int ID { get; set; }
        [Display(Name = "Retraining Program")]
        [Required(ErrorMessage ="You cannot leave name blank.")]
        [StringLength(70,ErrorMessage ="Name cannot be more than 70 characters.")]
        [DataType(DataType.MultilineText)]
        [DisplayFormat(NullDisplayText = "None")]
        public string Name { get; set; }
        public ICollection<Applicant> Applicants { get; set; }
    }
}
