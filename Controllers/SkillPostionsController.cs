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
    [Authorize]
    public class SkillPostionsController : Controller
    {
        private readonly JobBankContext _context;

        public SkillPostionsController(JobBankContext context)
        {
            _context = context;
        }

        // GET: SkillPostions
        public async Task<IActionResult> Index()
        {
            var jobBankContext = _context.SkillPostions.Include(s => s.Position).Include(s => s.Skill);
            return View(await jobBankContext.ToListAsync());
        }

        // GET: SkillPostions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skillPostion = await _context.SkillPostions
                .Include(s => s.Position)
                .Include(s => s.Skill)
                .FirstOrDefaultAsync(m => m.PositionID == id);
            if (skillPostion == null)
            {
                return NotFound();
            }

            return View(skillPostion);
        }

        // GET: SkillPostions/Create
        public IActionResult Create()
        {
            ViewData["PositionID"] = new SelectList(_context.Positions, "ID", "Description");
            ViewData["SkillID"] = new SelectList(_context.Skills, "ID", "Name");
            return View();
        }

        // POST: SkillPostions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SkillID,PositionID")] SkillPostion skillPostion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(skillPostion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PositionID"] = new SelectList(_context.Positions, "ID", "Description", skillPostion.PositionID);
            ViewData["SkillID"] = new SelectList(_context.Skills, "ID", "Name", skillPostion.SkillID);
            return View(skillPostion);
        }

        // GET: SkillPostions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skillPostion = await _context.SkillPostions.FindAsync(id);
            if (skillPostion == null)
            {
                return NotFound();
            }
            ViewData["PositionID"] = new SelectList(_context.Positions, "ID", "Description", skillPostion.PositionID);
            ViewData["SkillID"] = new SelectList(_context.Skills, "ID", "Name", skillPostion.SkillID);
            return View(skillPostion);
        }

        // POST: SkillPostions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SkillID,PositionID")] SkillPostion skillPostion)
        {
            if (id != skillPostion.PositionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(skillPostion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillPostionExists(skillPostion.PositionID))
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
            ViewData["PositionID"] = new SelectList(_context.Positions, "ID", "Description", skillPostion.PositionID);
            ViewData["SkillID"] = new SelectList(_context.Skills, "ID", "Name", skillPostion.SkillID);
            return View(skillPostion);
        }

        // GET: SkillPostions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skillPostion = await _context.SkillPostions
                .Include(s => s.Position)
                .Include(s => s.Skill)
                .FirstOrDefaultAsync(m => m.PositionID == id);
            if (skillPostion == null)
            {
                return NotFound();
            }

            return View(skillPostion);
        }

        // POST: SkillPostions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var skillPostion = await _context.SkillPostions.FindAsync(id);
            _context.SkillPostions.Remove(skillPostion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SkillPostionExists(int id)
        {
            return _context.SkillPostions.Any(e => e.PositionID == id);
        }
    }
}
