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

//Name: Ba Binh Luong,
//Done for: Project 2 Part A, B, and C
//Last Modified: 2020 October 09 3:00pm

namespace Bluong4_Job_Bank.Controllers
{
    [Authorize(Roles = "Admin, Supervisor")]
    public class RetrainingProgramsController : Controller
    {
        private readonly JobBankContext _context;

        public RetrainingProgramsController(JobBankContext context)
        {
            _context = context;
        }

        // GET: RetrainingPrograms
        public async Task<IActionResult> Index()
        {
            return View(await _context.RetrainingPrograms.ToListAsync());
        }

        // GET: RetrainingPrograms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retrainingProgram = await _context.RetrainingPrograms
                .FirstOrDefaultAsync(m => m.ID == id);
            if (retrainingProgram == null)
            {
                return NotFound();
            }

            return View(retrainingProgram);
        }

        // GET: RetrainingPrograms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RetrainingPrograms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] RetrainingProgram retrainingProgram)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(retrainingProgram);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: RetrainingPrograms.Name"))
                {
                    ModelState.AddModelError("Name", "Unable to create new Retraining Program. Retraining Name cannot be duplicate.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to create new Retraining Program. Try again, and if the problem persists please contact Administrator.");
                }

            }

            return View(retrainingProgram);
        }

        // GET: RetrainingPrograms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retrainingProgram = await _context.RetrainingPrograms.FindAsync(id);
            if (retrainingProgram == null)
            {
                return NotFound();
            }
            return View(retrainingProgram);
        }

        // POST: RetrainingPrograms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var retrainingProgramToUpdate = await _context.RetrainingPrograms.FindAsync(id);
            if (retrainingProgramToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<RetrainingProgram>(retrainingProgramToUpdate,"",
                r=>r.Name))
            {
                try
                {
                    
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RetrainingProgramExists(retrainingProgramToUpdate.ID))
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
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("", "Unable to save changes. Remember, you cannot have duplicate Name of program.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }

            }
            return View(retrainingProgramToUpdate);
        }

        // GET: RetrainingPrograms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var retrainingProgram = await _context.RetrainingPrograms
                .FirstOrDefaultAsync(m => m.ID == id);
            if (retrainingProgram == null)
            {
                return NotFound();
            }

            return View(retrainingProgram);
        }

        // POST: RetrainingPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var retrainingProgram = await _context.RetrainingPrograms.FindAsync(id);
            try
            {
                _context.RetrainingPrograms.Remove(retrainingProgram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {

                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to delete Retraining Program. You cannot delete Retraining Program already have applicant.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to delete Retraining Program. Try again, and if the problem persists please contact Administrator.");
                }
            }
            return View(retrainingProgram);
        }

        private bool RetrainingProgramExists(int id)
        {
            return _context.RetrainingPrograms.Any(e => e.ID == id);
        }
    }
}
