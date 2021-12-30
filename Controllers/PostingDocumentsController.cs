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
    [Authorize(Roles = "Admin, Supervisor")]
    public class PostingDocumentsController : Controller
    {
        private readonly JobBankContext _context;

        public PostingDocumentsController(JobBankContext context)
        {
            _context = context;
        }

        // GET: PostingDocuments
        public async Task<IActionResult> Index(string SearchString, int? PositionID)
        {
            var pQuery = (from p in _context.Positions
                          orderby p.Name
                          select p).ToList();
            ViewData["PositionID"] = new SelectList(pQuery, "ID", "Name");
            ViewData["Filtering"] = "";
            var postingDocuments = from p in _context.PostingDocuments
                .Include(p => p.Posting)
                .ThenInclude(p=>p.Position)
                select p;

            

            if (PositionID.HasValue)
            {
                postingDocuments = postingDocuments.Where(p => p.Posting.Position.ID == PositionID);
                ViewData["Filtering"] = " show";
            }

            if (!String.IsNullOrEmpty(SearchString))
            {
                postingDocuments = postingDocuments.Where(p => p.FileName.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = " show";
            }
            return View(await postingDocuments.ToListAsync());
        }

        // GET: PostingDocuments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postingDocument = await _context.PostingDocuments
                .Include(p => p.Posting)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (postingDocument == null)
            {
                return NotFound();
            }

            return View(postingDocument);
        }

        // GET: PostingDocuments/Create
        public IActionResult Create()
        {
            ViewData["PostingID"] = new SelectList(_context.Postings, "ID", "ID");
            return View();
        }

        // POST: PostingDocuments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostingID,ID,MimeType,FileName")] PostingDocument postingDocument)
        {
            if (ModelState.IsValid)
            {
                _context.Add(postingDocument);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostingID"] = new SelectList(_context.Postings, "ID", "ID", postingDocument.PostingID);
            return View(postingDocument);
        }

        // GET: PostingDocuments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postingDocument = await _context.PostingDocuments.FindAsync(id);
            if (postingDocument == null)
            {
                return NotFound();
            }
            ViewData["PostingID"] = new SelectList(_context.Postings, "ID", "ID", postingDocument.PostingID);
            return View(postingDocument);
        }

        // POST: PostingDocuments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostingID,ID,MimeType,FileName")] PostingDocument postingDocument)
        {
            if (id != postingDocument.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postingDocument);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostingDocumentExists(postingDocument.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostingID"] = new SelectList(_context.Postings, "ID", "ID", postingDocument.PostingID);
            return View(postingDocument);
        }

        // GET: PostingDocuments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postingDocument = await _context.PostingDocuments
                .Include(p => p.Posting)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (postingDocument == null)
            {
                return NotFound();
            }

            return View(postingDocument);
        }

        // POST: PostingDocuments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postingDocument = await _context.PostingDocuments.FindAsync(id);
            _context.PostingDocuments.Remove(postingDocument);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //private void PopulateDropDownList(PostingDocument postingDocument = null)
        //{
        //    var pQuery = from p in _context.Positions
        //                 orderby p.Name
        //                 select p;
        //    ViewData["PositionID"] = new SelectList(pQuery, "ID", "Name", postingDocument?.Posting.PositionID);
        //}

        public FileContentResult Download(int id)
        {
            var theFile = _context.PostingDocuments
                .Include(d => d.FileContent)
                .Where(f => f.ID == id)
                .SingleOrDefault();
            return File(theFile.FileContent.Content, theFile.MimeType, theFile.FileName);
        }

        private bool PostingDocumentExists(int id)
        {
            return _context.PostingDocuments.Any(e => e.ID == id);
        }
    }
}
