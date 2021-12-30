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
    public class Posting: Auditable,IValidatableObject
    {
        public Posting()
        {
            this.Applications = new HashSet<Application>();
            this.PostingDocuments = new HashSet<PostingDocument>();
        }
        public int ID { get; set; }
        [Required(ErrorMessage = "You cannot leave posting number blank.")]
        [Display(Name = "Number of Positions Opening")]
        public uint NumberOpen { get; set; }
        [Display(Name = "Closing Date")]
        [Required(ErrorMessage = "You must choose closing date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode = true)]
        public DateTime ClosingDate { get; set; }
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "You must choose position for this job posting.")]
        [Display(Name = "Job Posting")]
        public string PositionClose
        {
            get
            {
                return (string.IsNullOrEmpty(Position?.Name) ? "" : Position?.Name + " ") +
                    " - Closing: " + ClosingDate.ToString("yyyy-MM-dd");
            }
        }
        public int PositionID { get; set; }
        public Position Position { get; set; }
        public ICollection<Application> Applications { get; set; }
        public ICollection<PostingDocument> PostingDocuments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(ClosingDate.Date < DateTime.Today)
            {
                yield return new ValidationResult("Closing Date can not be in the past.", new[] { "ClosingDate" });
            }
            if(StartDate.GetValueOrDefault() > ClosingDate)
            {
                yield return new ValidationResult("Start Date cannot be earlier than Closing day", new[] { "StartDate" });
            }
        }
    }
}
