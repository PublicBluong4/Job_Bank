using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bluong4_Job_Bank.Data;
using Bluong4_Job_Bank.Models;
using Bluong4_Job_Bank.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Bluong4_Job_Bank.Utilities;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

//Name: Ba Binh Luong,
//Done for: Project 2 Part A, B, and C
//Last Modified: 2020 October 09 3:00pm

namespace Bluong4_Job_Bank.Controllers
{
    [Authorize(Roles = "Admin, Supervisor, Staff")]
    public class ApplicantsController : Controller
    {
        private readonly JobBankContext _context;

        public ApplicantsController(JobBankContext context)
        {
            _context = context;
        }

        // GET: Applicants
        public async Task<IActionResult> Index(string SearchString, int? PostingID, string SearchEmail, int? SkillID, int? page,
            int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Applicant")
        {
            var aQuery = (from a in _context.Postings.Include(p => p.Position)
                          orderby a.Position.Name, a.ClosingDate
                          select a).ToList();
            var sQuery = (from s in _context.Skills
                          orderby s.Name
                          select s).ToList();
            ViewData["PostingID"] = new SelectList(aQuery, "ID", "PositionClose");
            ViewData["SkillID"] = new SelectList(sQuery, "ID", "Name");
            //PopulateDDLJobPost();
            ViewData["Filtering"] = "";
            var applicants = from a in _context.Applicants
                .Include(app => app.ApplicantDocuments)
                .Include(app=>app.ApplicantPhoto).ThenInclude(app=>app.PhotoContentThumb)
                .Include(app=>app.RetrainingProgram)
                .Include(app => app.Applications).ThenInclude(app => app.Posting).ThenInclude(app => app.Position)
                .Include(s => s.ApplicantSkills).ThenInclude(s => s.Skill)
                select a;
            if(PostingID.HasValue)
            {
                applicants = applicants.Where(a => a.Applications.Any(p => p.PostingID == PostingID));
                ViewData["Filtering"] = " show";
            }
            if(SkillID.HasValue)
            {
                applicants = applicants.Where(a => a.ApplicantSkills.Any(s => s.SkillID == SkillID));
                ViewData["Filtering"] = " show";
            }
            if(!String.IsNullOrEmpty(SearchString))
            {
                applicants = applicants.Where(a => a.MiddleName.ToUpper().Contains(SearchString.ToUpper()) || a.LastName.ToUpper().Contains(SearchString.ToUpper()) || a.FirstName.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = " show";
            }
            if(!String.IsNullOrEmpty(SearchEmail))
            {
                applicants = applicants.Where(a => a.Email.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = " show";
            }

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted so lets sort!
            {
                if (actionButton != "Filter")//Change of sort is requested
                {
                    page = 1;
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }


            //Set for sort field

            if (sortField == "Email")
            {
                if (sortDirection == "asc")
                {
                    applicants = applicants
                        .OrderBy(a => a.Email);
                }
                else
                {
                    applicants = applicants
                        .OrderByDescending(a => a.Email);
                }
            }
            else if (sortField == "Phone")
            {
                if (sortDirection == "asc")
                {
                    applicants = applicants
                        .OrderBy(a => a.Phone);
                }
                else
                {
                    applicants = applicants
                        .OrderByDescending(a => a.Phone);
                }
            }
            else //Sorting by Applicant Name
            {
                if (sortDirection == "asc")
                {
                    applicants = applicants
                        .OrderBy(a => a.LastName)
                        .ThenBy(a => a.FirstName);
                }
                else
                {
                    applicants = applicants
                        .OrderByDescending(a => a.LastName)
                        .ThenByDescending(a => a.FirstName);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            int pageSize;//This is the value we will pass to PaginatedList
            if (pageSizeID.HasValue)
            {
                //Value selected from DDL so use and save it to Cookie
                pageSize = pageSizeID.GetValueOrDefault();
                CookieHelper.CookieSet(HttpContext, "pageSizeValue", pageSize.ToString(), 30);
            }
            else
            {
                //Not selected so see if it is in Cookie
                pageSize = Convert.ToInt32(HttpContext.Request.Cookies["pageSizeValue"]);
            }
            pageSize = (pageSize == 0) ? 5 : pageSize;//Neither Selected or in Cookie so go with default
            ViewData["pageSizeID"] =
                new SelectList(new[] { "3", "5", "10", "20", "30", "40", "50", "100", "500" }, pageSize.ToString());


            var pagedData = await PaginatedList<Applicant>.CreateAsync(applicants.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Applicants/Details/5
        public async Task<IActionResult> Details(int? id, string returnURL)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Get the URL of the page that send us here
            if (String.IsNullOrEmpty(returnURL))
            {
                returnURL = Request.Headers["Referer"].ToString();
            }
            ViewData["returnURL"] = returnURL;

            //Include application which applicant were submited.
            var applicant = await _context.Applicants
                .Include(app => app.ApplicantPhoto).ThenInclude(app => app.PhotoContentFull)
                .Include(app => app.RetrainingProgram)
                .Include(app => app.Applications).ThenInclude(app=>app.Posting).ThenInclude(app=>app.Position)
                .Include(s=>s.ApplicantSkills).ThenInclude(s=>s.Skill)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (applicant == null)
            {
                return NotFound();
            }
            return View(applicant);
        }

        // GET: Applicants/Create
        public IActionResult Create(string returnURL)
        {
            //Get the URL of the page that send us here
            if (String.IsNullOrEmpty(returnURL))
            {
                returnURL = Request.Headers["Referer"].ToString();
            }
            ViewData["returnURL"] = returnURL;
            //Add all (unchecked) Skill to ViewBag
            var applicant = new Applicant();
            PopulateAssignedSkillData(applicant);
            PopulateDropDownList();
            return View();
        }

        // POST: Applicants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,SIN,Phone,Email,RetrainingProgramID")] Applicant applicant, string[] selectedOptions
            , IFormFile thePicture, string returnURL, List<IFormFile> theFiles)
        {
            ViewData["returnURL"] = returnURL;
            try
            {
                //Add the selected skills
                if(selectedOptions != null)
                {
                    foreach(var skill in selectedOptions)
                    {
                        var skillToAdd = new ApplicantSkill { ApplicantID = applicant.ID, SkillID = int.Parse(skill) };
                        applicant.ApplicantSkills.Add(skillToAdd);
                    }
                }
                if (ModelState.IsValid)
                {
                    await AddPicture(applicant, thePicture);
                    await AddDocumentsAsync(applicant, theFiles);
                    _context.Add(applicant);
                    await _context.SaveChangesAsync();
                    //If no referrer then go back to index
                    if (String.IsNullOrEmpty(returnURL))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction("Details", new { applicant.ID, returnURL });
                    }

                }
            }
            catch(RetryLimitExceededException)
            {
                ModelState.AddModelError("", "unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {

                if(dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Applicants.Email"))
                {
                    ModelState.AddModelError("Email", "Unable to create new Applicant. Email address exist already.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to create new Applicant. Try again, and if the problem persists please contact Administrator.");
                }
            }
            PopulateAssignedSkillData(applicant);
            PopulateDropDownList(applicant);
            return View(applicant);
        }

        // GET: Applicants/Edit/5
        public async Task<IActionResult> Edit(int? id, string returnURL)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Get the URL of the page that send us here
            if (String.IsNullOrEmpty(returnURL))
            {
                returnURL = Request.Headers["Referer"].ToString();
            }
            ViewData["returnURL"] = returnURL;

            var applicant = await _context.Applicants
                .Include(app=>app.ApplicantDocuments)
                .Include(app => app.ApplicantPhoto).ThenInclude(app => app.PhotoContentFull)
                .Include(a=>a.RetrainingProgram)
                .Include(a=>a.ApplicantSkills).ThenInclude(a=>a.Skill)
                .AsNoTracking()
                .SingleOrDefaultAsync(a=>a.ID ==id);
            if (applicant == null)
            {
                return NotFound();
            }
            PopulateAssignedSkillData(applicant);
            PopulateDropDownList(applicant);
            return View(applicant);
        }

        // POST: Applicants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] SelectedOptions, 
            string chkRemoveImage, IFormFile thePicture, string returnURL, List<IFormFile> theFiles, Byte[] RowVersion)
        {
            ViewData["returnURL"] = returnURL;
            //Go get the applicant to update
            var applicantToUpdate = await _context.Applicants
                .Include(app => app.ApplicantDocuments)
                .Include(app => app.ApplicantPhoto).ThenInclude(app => app.PhotoContentFull)
                .Include(a => a.RetrainingProgram)
                .Include(a=>a.ApplicantSkills).ThenInclude(a=>a.Skill)
                .SingleOrDefaultAsync(app=>app.ID ==id);
            if(applicantToUpdate == null)
            {
                return NotFound();
            }
            //Update the skill history
            UpdateApplicantSkills(SelectedOptions, applicantToUpdate);

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(applicantToUpdate).Property("RowVersion").OriginalValue = RowVersion;

            //Try update Applicant with the values posted
            if (await TryUpdateModelAsync<Applicant>(applicantToUpdate,"",
                app=>app.FirstName, app=>app.MiddleName, app=>app.LastName, app=>app.SIN, app=>app.Phone, app=>app.Email,app=>app.RetrainingProgramID))
            {
                try
                {
                    //For the image
                    if (chkRemoveImage != null)
                    {
                        applicantToUpdate.ApplicantPhoto = null;
                    }
                    else
                    {
                        await AddPicture(applicantToUpdate, thePicture);
                    }
                    await AddDocumentsAsync(applicantToUpdate, theFiles);
                    await _context.SaveChangesAsync();
                    //If no referrer then go back to index
                    if (String.IsNullOrEmpty(returnURL))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction("Details", new { applicantToUpdate.ID, returnURL });
                    }
                }
                catch(RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException ex)// Added for concurrency
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Applicant)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The Applicant was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Applicant)databaseEntry.ToObject();
                        if (databaseValues.FirstName != clientValues.FirstName)
                            ModelState.AddModelError("FirstName", "Current value: "
                                + databaseValues.FirstName);
                        if (databaseValues.MiddleName != clientValues.MiddleName)
                            ModelState.AddModelError("MiddleName", "Current value: "
                                + databaseValues.MiddleName);
                        if (databaseValues.LastName != clientValues.LastName)
                            ModelState.AddModelError("LastName", "Current value: "
                                + databaseValues.LastName);
                        if (databaseValues.SIN != clientValues.SIN)
                            ModelState.AddModelError("SIN", "Current value: "
                                + databaseValues.SIN);
                        if (databaseValues.Phone != clientValues.Phone)
                            ModelState.AddModelError("Phone", "Current value: "
                                + String.Format("{0:(###) ###-####}", databaseValues.Phone));
                        if (databaseValues.Email != clientValues.Email)
                            ModelState.AddModelError("Email", "Current value: "
                                + databaseValues.Email);
                        //A little extra work for the nullable foreign key.  No sense going to the database and asking for something
                        //we already know is not there.
                        if (databaseValues.RetrainingProgramID != clientValues.RetrainingProgramID)
                        {
                            if (databaseValues.RetrainingProgramID.HasValue)
                            {
                                RetrainingProgram databaseMedicalTrial = await _context.RetrainingPrograms.SingleOrDefaultAsync(i => i.ID == databaseValues.RetrainingProgramID);
                                ModelState.AddModelError("RetrainingProgramID", $"Current value: {databaseMedicalTrial?.Name}");
                            }
                            else

                            {
                                ModelState.AddModelError("RetrainingProgramID", $"Current value: None");
                            }
                        }
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to List' hyperlink.");
                        applicantToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
                catch (DbUpdateException dex)
                {

                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Applicants.Email"))
                    {
                        ModelState.AddModelError("Email", "Unable to update Applicant. Email address exist already.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to update Applicant. Try again, and if the problem persists please contact Administrator.");
                    }
                }

            }
            //Validation Error so give user another chance
            PopulateAssignedSkillData(applicantToUpdate);
            PopulateDropDownList(applicantToUpdate);
            return View(applicantToUpdate);
        }

        // GET: Applicants/Delete/5
        [Authorize(Roles = "Admin, Supervisor")]
        public async Task<IActionResult> Delete(int? id, string returnURL)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Get the URL of the page that send us here
            if (String.IsNullOrEmpty(returnURL))
            {
                returnURL = Request.Headers["Referer"].ToString();
            }
            ViewData["returnURL"] = returnURL;

            var applicant = await _context.Applicants
                .Include(a=>a.RetrainingProgram)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (applicant == null)
            {
                return NotFound();
            }

            return View(applicant);
        }

        // POST: Applicants/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin, Supervisor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string returnURL)
        {
            var applicant = await _context.Applicants
                .Include(a => a.RetrainingProgram)
                .FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                _context.Applicants.Remove(applicant);
                await _context.SaveChangesAsync();
                //If no referrer then go back to index
                if (String.IsNullOrEmpty(returnURL))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return Redirect(returnURL);
                }
            }
            catch (DbUpdateException dex)
            {

                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to delete Applicant. You cannot delete Applicant already have application.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to delete Applicant. Try again, and if the problem persists please contact Administrator.");
                }
            }
            return View(applicant);
            
        }
        private async Task AddPicture(Applicant applicant, IFormFile thePicture)
        {
            //get the picture and save it with the Applicant
            if (thePicture != null)
            {
                string mimeType = thePicture.ContentType;
                long fileLength = thePicture.Length;
                if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                {
                    if (mimeType.Contains("image"))
                    {
                        ApplicantPhoto p = new ApplicantPhoto
                        {
                            FileName = Path.GetFileName(thePicture.FileName)
                        };
                        using (var memoryStream = new MemoryStream())
                        {
                            await thePicture.CopyToAsync(memoryStream);
                            p.PhotoContentFull.Content = memoryStream.ToArray();
                            p.PhotoContentFull.MimeType = mimeType;
                            p.PhotoContentThumb.Content = ResizeImage.shrinkImagePNG(p.PhotoContentFull.Content);
                            p.PhotoContentThumb.MimeType = "image/png";
                        }
                        applicant.ApplicantPhoto = p;
                    }
                }
            }
        }
        private void PopulateAssignedSkillData(Applicant applicant)
        {
            var allSkillOptions = _context.Skills;
            var currentSkillOptionIDs = new HashSet<int>(applicant.ApplicantSkills.Select(s => s.SkillID));
            var checkBoxes = new List<OptionVM>();
            foreach(var option in allSkillOptions)
            {
                checkBoxes.Add(new OptionVM
                {
                    ID = option.ID,
                    DisplayText = option.Name,
                    Assigned = currentSkillOptionIDs.Contains(option.ID)
                }
                ) ;

            }
            ViewData["SkillOptions"] = checkBoxes;
        }
        private void UpdateApplicantSkills(string[] selectedOptions, Applicant applicantToUpdate)
        {
            if(selectedOptions == null)
            {
                applicantToUpdate.ApplicantSkills = new List<ApplicantSkill>();
                return;
            }
            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var applicantOptionsHS = new HashSet<int>
                (applicantToUpdate.ApplicantSkills.Select(s => s.SkillID));
            foreach(var option in _context.Skills)
            {
                if(selectedOptionsHS.Contains(option.ID.ToString()))
                {
                    if(!applicantOptionsHS.Contains(option.ID))
                    {
                        applicantToUpdate.ApplicantSkills.Add(new ApplicantSkill { ApplicantID = applicantToUpdate.ID, SkillID = option.ID });
                    }
                }
                else
                {
                    if(applicantOptionsHS.Contains(option.ID))
                    {
                        ApplicantSkill skillToRemove = applicantToUpdate.ApplicantSkills.SingleOrDefault(s => s.SkillID == option.ID);
                        _context.Remove(skillToRemove);
                    }
                }
            }
        }

        private void PopulateDropDownList(Applicant applicant = null)
        {
            var dQuery = from d in _context.RetrainingPrograms
                         orderby d.Name
                         select d;
            ViewData["RetrainingProgramID"] = new SelectList(dQuery, "ID", "Name", applicant?.RetrainingProgramID);
            //var jpQuery = _context.Applicants
            //    .Include(a => a.Applications)
            //    .ThenInclude(a => a.Posting)
            //    .ThenInclude(a => a.ID);
            //ViewData["Filtering"] = new SelectList(jpQuery, "ID","Name", applicant?.Applications.Posting.ID)
        }
        private void PopulateDDLJobPost(Application application = null)
        {
            var jQuery = from j in _context.Applications
                         .Include(a => a.PostingID)
                         select j;
            ViewData["PostingID"] = new SelectList(jQuery, "ID","Name", application?.PostingID);
        }

        private bool ApplicantExists(int id)
        {
            return _context.Applicants.Any(e => e.ID == id);
        }

        public FileContentResult Download(int id)
        {
            var theFile = _context.ApplicantDocuments
                .Include(d => d.FileContent)
                .Where(f => f.ID == id)
                .SingleOrDefault();
            return File(theFile.FileContent.Content, theFile.MimeType, theFile.FileName);
        }

        private async Task AddDocumentsAsync(Applicant applicant, List<IFormFile> theFiles)
        {
            foreach (var f in theFiles)
            {
                if (f != null)
                {
                    string mimeType = f.ContentType;
                    string fileName = Path.GetFileName(f.FileName);
                    long fileLength = f.Length;
                    //Note: you could filter for mime types if you only want to allow
                    //certain types of files.  I am allowing everything.
                    if (!(fileName == "" || fileLength == 0))//Looks like we have a file!!!
                    {
                        ApplicantDocument d = new ApplicantDocument();
                        using (var memoryStream = new MemoryStream())
                        {
                            await f.CopyToAsync(memoryStream);
                            d.FileContent.Content = memoryStream.ToArray();
                        }
                        d.MimeType = mimeType;
                        d.FileName = fileName;
                        applicant.ApplicantDocuments.Add(d);
                    };
                }
            }
        }
    }
}
