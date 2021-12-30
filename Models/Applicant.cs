using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bluong4_Job_Bank.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

//Name: Ba Binh Luong,
//Done for: Project 2 Part A, B, and C
//Last Modified: 2020 October 09 3:00pm

namespace Bluong4_Job_Bank.Models
{
    public class Applicant: Auditable
    {
        public Applicant()
        {
            this.ApplicantSkills = new HashSet<ApplicantSkill>();
            this.Applications = new HashSet<Application>();
            this.ApplicantDocuments = new HashSet<ApplicantDocument>();
            this.ApplicantPhoto = new ApplicantPhoto();
        }
        public int ID { get; set; }
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave First Name bank.")]
        [StringLength(255, ErrorMessage = "First Name cannot be more than 50 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(255, ErrorMessage = "Middle Name cannot be more than 30 characters long.")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave Last Name bank.")]
        [StringLength(255, ErrorMessage = "Last Name cannot be more than 50 characters long.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You cannot leave SIN number blank")]
        [RegularExpression("^\\d{9}$",ErrorMessage ="The SIN number must exactly 9 digits.")]

        public int SIN { get; set; }

        [Required(ErrorMessage ="Phone number is required, you cannot leave it bank.")]
        [RegularExpression("^\\d{10}$",ErrorMessage ="The Phone number must exactly 10 digits.")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString ="{0:(###) ###-####}", ApplyFormatInEditMode = false)]
        public Int64 Phone { get; set; }

        [Required(ErrorMessage ="You cannot leave Email blank.")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Applicant Name")]

        public string FullName
        {
            get
            {
                return "Mr/Ms " + FirstName + (string.IsNullOrEmpty(MiddleName) ? " " :
                    (" " + (char?)MiddleName[0] + ". ").ToUpper()) + LastName;
            }
        }

        public string FormalName
        {
            get
            {
                return FirstName + (string.IsNullOrEmpty(MiddleName) ? "" :
                    (" " + (char?)MiddleName[0] + ". ").ToUpper()) + LastName;
            }
        }


        public ICollection<Application> Applications { get; set; }

        //One applicant can have many skill
        [Display(Name = "Skills")]
        public ICollection<ApplicantSkill> ApplicantSkills { get; set; }
        [Display(Name = "Retraining Program")]
        public int? RetrainingProgramID { get; set; }
        [Display(Name = "Retraining Program")]
        public RetrainingProgram RetrainingProgram { get; set; }
        public ApplicantPhoto ApplicantPhoto { get; set; }
        public ICollection<ApplicantDocument> ApplicantDocuments { get; set; }

        [ScaffoldColumn(false)]
        [Timestamp]
        public Byte[] RowVersion { get; set; }//Added for concurrency
    }
}
