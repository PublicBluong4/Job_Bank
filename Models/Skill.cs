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
    public class Skill
    {
        public Skill()
        {
            this.ApplicantSkills = new HashSet<ApplicantSkill>();
            this.SkillPostions = new HashSet<SkillPostion>();
        }
        public int ID { get; set; }

        [Display(Name = "Skills Name")]
        [Required(ErrorMessage ="You cannot leave skill name blank.")]
        [StringLength(50,ErrorMessage ="Skill name cannot be more than 50 characters.")]
        public string Name { get; set; }

        //One skill can belong to many applicant
        public ICollection<ApplicantSkill> ApplicantSkills { get; set; }
        public ICollection<SkillPostion> SkillPostions { get; set; }

    }
}
