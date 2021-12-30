using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bluong4_Job_Bank.Data;
using Bluong4_Job_Bank.Models;
using Bluong4_Job_Bank.Utilities;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;
using Bluong4_Job_Bank.ViewModels;

//Name: Ba Binh Luong,
//Done for: Project 2 Part A, B, and C
//Last Modified: 2020 October 09 3:00pm

namespace Bluong4_Job_Bank.Controllers
{
    [Authorize(Roles = "Admin, Supervisor, Staff")]
    public class PostingsController : Controller
    {
        //for sending email
        private readonly IMyEmailSender _emailSender;
        private readonly JobBankContext _context;

        public PostingsController(JobBankContext context, IMyEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: Postings
        public async Task<IActionResult> Index(int? page, int? pageSizeID)
        {
            var posting = _context.Postings
                .Include(p => p.Position)
                .Include(p=>p.PostingDocuments);
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
            pageSize = (pageSize == 0) ? 3 : pageSize;//Neither Selected or in Cookie so go with default
            ViewData["pageSizeID"] =
                new SelectList(new[] { "3", "5", "10", "20", "30", "40", "50", "100", "500" }, pageSize.ToString());
            var pagedData = await PaginatedList<Posting>.CreateAsync(posting.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }


        // GET/POST: JobPosting/Notification/5
        public async Task<IActionResult> Notification(int? id, string Subject, string emailContent, string PositionClose)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["id"] = id;
            ViewData["PositionClose"] = PositionClose;

            if (string.IsNullOrEmpty(Subject) || string.IsNullOrEmpty(emailContent))
            {
                ViewData["Message"] = "You must enter both a Subject and some message Content before sending the message.";
            }
            else
            {
                int folksCount = 0;
                try
                {
                    //Send a Notice.
                    //Send a Notice.
                    List<EmailAddress> folks = (from p in _context.Applications
                                                .Include(a=>a.Applicant)
                                                .Include(a=>a.Posting)
                                                where p.PostingID == id
                                                select new EmailAddress
                                                {
                                                    Name = p.Applicant.FullName,
                                                    Address = p.Applicant.Email
                                                }).ToList();

                    folksCount = folks.Count();
                    if (folksCount > 0)
                    {
                        var msg = new EmailMessage()
                        {
                            ToAddresses = folks,
                            Subject = Subject,
                            Content = "<p>" + emailContent + "</p><p>Please access the <strong>Niagara College</strong> web site to review.</p>"

                        };
                        await _emailSender.SendToManyAsync(msg);
                        ViewData["Message"] = "Message sent to " + folksCount + " Applicant"
                            + ((folksCount == 1) ? "." : "s.");
                    }
                    else
                    {
                        ViewData["Message"] = "Message NOT sent!  No Applicant in Job Posting.";
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = ex.GetBaseException().Message;
                    ViewData["Message"] = "Error: Could not send email message to the " + folksCount + " Applicant"
                        + ((folksCount == 1) ? "" : "s") + " in the Job Posting.";
                }
            }
            return View();
        }


        // GET: Postings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var posting = await _context.Postings
                .Include(p=>p.PostingDocuments)
                .Include(p => p.Position)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (posting == null)
            {
                return NotFound();
            }

            return View(posting);
        }

        // GET: Postings/Create
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        

        // POST: Postings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,NumberOpen,ClosingDate,StartDate,PositionID")] Posting posting, 
            List<IFormFile> theFiles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AddDocumentsAsync(posting, theFiles);
                    _context.Add(posting);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {

                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Postings.PositionID, Postings.ClosingDate"))
                {
                    ModelState.AddModelError("", "Unable to create new Posting. Position and Closing Date must unique compsite constraint.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to create new Posting. Try again, and if the problem persists please contact Administrator.");
                }
            }

            PopulateDropDownLists(posting);
            return View(posting);
        }

        // GET: Postings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var posting = await _context.Postings
                .Include(p => p.PostingDocuments)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.ID == id);
            if (posting == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(posting);
            return View(posting);
        }

        // POST: Postings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, List<IFormFile> theFiles)
        {
            //Get posting to update
            var postingToUpdate = await _context.Postings
                .Include(p => p.PostingDocuments)
                .SingleOrDefaultAsync(p => p.ID == id);
            if (postingToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Posting>(postingToUpdate, "",
                p=>p.NumberOpen, p=>p.ClosingDate, p=>p.StartDate, p=>p.PositionID))
            {
                try
                {
                    await AddDocumentsAsync(postingToUpdate, theFiles);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostingExists(postingToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {

                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Postings.PositionID, Postings.ClosingDate"))
                    {
                        ModelState.AddModelError("", "Unable to create new Posting. Position and Closing Date must unique compsite constraint.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to create new Posting. Try again, and if the problem persists please contact Administrator.");
                    }
                }

            }
            PopulateDropDownLists(postingToUpdate);
            return View(postingToUpdate);
        }

        // GET: Postings/Delete/5
        [Authorize(Roles = "Admin, Supervisor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var posting = await _context.Postings
                .Include(p => p.Position)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (posting == null)
            {
                return NotFound();
            }

            return View(posting);
        }

        // POST: Postings/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin, Supervisor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var posting = await _context.Postings.FindAsync(id);
            try
            {
                _context.Postings.Remove(posting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {

                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to delete Posting. You cannot delete Posting already have Application.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to delete Posting. Try again, and if the problem persists please contact Administrator.");
                }
            }
            return View(posting);

        }

        public FileContentResult Download(int id)
        {
            var theFile = _context.PostingDocuments
                .Include(d => d.FileContent)
                .Where(f => f.ID == id)
                .SingleOrDefault();
            return File(theFile.FileContent.Content, theFile.MimeType, theFile.FileName);
        }

        private async Task AddDocumentsAsync(Posting posting, List<IFormFile> theFiles)
        {
            foreach (var f in theFiles)
            {
                if (f != null)
                {
                    string mimeType = f.ContentType;
                    string fileName = Path.GetFileName(f.FileName);
                    long fileLength = f.Length;
                    if (!(fileName == "" || fileLength == 0))
                    {
                        PostingDocument d = new PostingDocument();
                        using (var memoryStream = new MemoryStream())
                        {
                            await f.CopyToAsync(memoryStream);
                            d.FileContent.Content = memoryStream.ToArray();
                        }
                        d.MimeType = mimeType;
                        d.FileName = fileName;
                        posting.PostingDocuments.Add(d);
                    };
                }
            }
        }
        private void PopulateDropDownLists(Posting posting = null)
        {
            var pQuery = from p in _context.Positions
                         orderby p.Name
                         select p;
            ViewData["PositionID"] = new SelectList(pQuery, "ID", "Name", posting?.PositionID);
        }
        private bool PostingExists(int id)
        {
            return _context.Postings.Any(e => e.ID == id);
        }
    }
}
