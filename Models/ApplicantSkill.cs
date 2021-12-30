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
    public class ApplicantSkill
    {

        [Required(ErrorMessage ="You cannot leave SkillID blank.")]
        public int SkillID { get; set; }
        public Skill Skill { get; set; }
        [Required(ErrorMessage ="You cannot leave ApplicantID blank.")]
        public int ApplicantID { get; set; }
        public Applicant Applicant { get; set; }

    }
}
