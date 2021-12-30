using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

//Name: Ba Binh Luong,
//Done for: Project 1 Part A and B
//Last Modified: 2020 September 28 6:00pm

namespace Bluong4_Job_Bank.Models
{
    public class Position: Auditable
    {
        public Position()
        {
            this.Postings = new HashSet<Posting>();
            this.SkillPostions = new HashSet<SkillPostion>();
        }
        public int ID { get; set; }
        [Display(Name = "Position Name")]
        [Required(ErrorMessage ="You cannot leave Position name blank.")]
        [StringLength(255,ErrorMessage = "Posistion name cannot be more than 255 characters long.")]
        public string Name { get; set; }
        [Display(Name = "Position Description")]
        [Required(ErrorMessage ="You cannot leave position description blank.")]
        [StringLength(511,ErrorMessage = "Position description cannot be less than 20 or more than 511 characters long.",MinimumLength = 20)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required(ErrorMessage = "You cannot leave salary blank.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(9,2)")]
        [Range(0,9999999.99,ErrorMessage ="Salary must in range 0 - 9999999.99 $.")]
        public decimal Salary { get; set; }
        [Required(ErrorMessage = "You must choose Occupation for this position.")]
        public int OccupationID { get; set; }

        public Occupation Occupation { get; set; }
        public ICollection<Posting> Postings { get; set; }
        public ICollection<SkillPostion> SkillPostions { get; set; }


    }
}
