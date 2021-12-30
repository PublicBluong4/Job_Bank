using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bluong4_Job_Bank.Models
{
    public class SkillPostion
    {
        [Required(ErrorMessage ="You cannot leave SkillID blank.")]
        public int SkillID { get; set; }
        public Skill Skill { get; set; }
        [Required(ErrorMessage ="You cannot leave PositionID blank.")]
        public int PositionID { get; set; }
        public Position Position { get; set; }
    }
}
