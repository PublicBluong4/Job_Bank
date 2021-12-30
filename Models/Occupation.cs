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
    public class Occupation: Auditable
    {
        public Occupation()
        {
            this.Positions = new HashSet<Position>();
        }
        public int ID { get; set; }
        [Display(Name = "Occupation Title")]
        [Required(ErrorMessage ="You cannot leave the Title blank.")]
        [StringLength(50,ErrorMessage ="Title cannot be more than 50 characters long.")]
        public string Title { get; set; }
        public ICollection<Position> Positions { get; set; }
    }
}
