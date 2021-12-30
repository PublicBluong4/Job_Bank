using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bluong4_Job_Bank.Data;
using Bluong4_Job_Bank.Models;
using Microsoft.AspNetCore.Authorization;

namespace Bluong4_Job_Bank.Controllers
{
    [Authorize(Roles = "Admin, Supervisor, Staff")]
    public class ApplicationsController : Controller
    {
        private readonly JobBankContext _context;

        public ApplicationsController(JobBankContext context)
        {
            _context = context;
        }

        // GET: Applications
        public async Task<IActionResult> Index()
        {
            var jobBankContext = _context.Applications.Include(a => a.Applicant).Include(a => a.Posting);
            return View(await jobBankContext.ToListAsync());
        }

        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.Applicant)
                .Include(a => a.Posting)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // GET: Applications/Create
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }


        

        // POST: Applications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Comments,PostingID,ApplicantID")] Application application)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(application);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Applications.PostingID, Applications.ApplicantID"))
                {
                    ModelState.AddModelError("", "Unable to create new Application. One Applicant cannot apply to one posting job twice");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to create new Application. Try again, and if the problem persists please contact Administrator.");
                }

            }
            
            
            PopulateDropDownLists(application);
            return View(application);
        }

        // GET: Applications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(application);
            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            //Go get Application to update
            var applicationToUpdate = await _context.Applications.SingleOrDefaultAsync(app => app.ID == id);
            if (applicationToUpdate == null)
            {
                return NotFound();
            }

            //Try to update Application with values posted
            if (await TryUpdateModelAsync<Application>(applicationToUpdate, "",
                app => app.Comments, app => app.PostingID, app => app.ApplicantID))
            {
                try
                {
                    
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationExists(applicationToUpdate.ID))
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
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Applications.PostingID, Applications.ApplicantID"))
                    {
                        ModelState.AddModelError("", "Unable to update Application. One Applicant cannot apply to one posting job twice");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to update Application. Try again, and if the problem persists please contact Administrator.");
                    }

                }
            }
            PopulateDropDownLists(applicationToUpdate);
            return View(applicationToUpdate);
        }

        // GET: Applications/Delete/5
        [Authorize(Roles = "Admin, Supervisor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.Applicant)
                .Include(a => a.Posting)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin, Supervisor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private void PopulateDropDownLists(Application application = null)
        {
            var appQuery = from app in _context.Applicants
                           orderby app.LastName, app.FirstName
                           select app;
            ViewData["ApplicantID"] = new SelectList(appQuery, "ID", "FullName", application?.ApplicantID);
            var posQuery = from pos in _context.Postings
                           orderby pos.Position
                           select pos;
            ViewData["PostingID"] = new SelectList(posQuery, "ID", "ID", application?.PostingID);
        }
        private bool ApplicationExists(int id)
        {
            return _context.Applications.Any(e => e.ID == id);
        }
    }
}
