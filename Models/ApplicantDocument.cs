using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bluong4_Job_Bank.Models
{
    public class ApplicantDocument: UploadedFile
    {
        [Display(Name = "Cover Letter")]
        public int ApplicantID { get; set; }
        public Applicant Applicant { get; set; }
    }
}
