using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


//Name: Ba Binh Luong,
//Done for: Project 1 Part A and B
//Last Modified: 2020 September 28 6:00pm
namespace Bluong4_Job_Bank.Models
{
    public class Application: Auditable
    {
        public int ID { get; set; }
        [Required(ErrorMessage ="You cannot leave comments blank.")]
        [StringLength(200,ErrorMessage ="Comments must have at least 20 characters but it cannot more than 200 characters.",MinimumLength =20)]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Required(ErrorMessage ="Posting ID is required.")]
        
        public int PostingID { get; set; }
        [Display(Name = "Job Posting ID")]
        public Posting Posting { get; set; }

        [Required(ErrorMessage ="ApplicantID is required.")]
        public int ApplicantID { get; set; }

        [Display(Name = "Applicant Name")]
        public Applicant Applicant { get; set; }

    }
}
