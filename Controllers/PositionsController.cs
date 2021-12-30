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
using Microsoft.AspNetCore.Authorization;

namespace Bluong4_Job_Bank.Controllers
{
    [Authorize(Roles = "Admin, Supervisor, Staff")]
    public class PositionsController : Controller
    {
        private readonly JobBankContext _context;

        public PositionsController(JobBankContext context)
        {
            _context = context;
        }

        // GET: Positions
        public async Task<IActionResult> Index(int? page, int? pageSizeID)
        {
            var positions = from p in _context.Positions.Include(p => p.Occupation)
                            .Include(p=>p.SkillPostions)
                            .ThenInclude(p=>p.Skill)
                            select p;

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
            var pagedData = await PaginatedList<Position>.CreateAsync(positions
                .OrderBy(p=>p.Name)
                .AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        // GET: Positions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .Include(p => p.Occupation)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // GET: Positions/Create
        [Authorize(Roles = "Admin, Supervisor")]
        public IActionResult Create()
        {
            Position position = new Position();
            PopulateAssignedSkillData(position);
            PopulateDropDownLists();
            return View();
        }

        

        // POST: Positions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Supervisor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,Salary,OccupationID")] Position position, string[] selectedOptions)
        {
            try
            {
                UpdateSkillPositions(selectedOptions, position);
                if (ModelState.IsValid)
                {
                    _context.Add(position);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {

                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Positions.Name"))
                {
                    ModelState.AddModelError("Name", "Unable to create new Position. Position Name cannot duplicate.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to create new Position. Try again, and if the problem persists please contact Administrator.");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateAssignedSkillData(position);
            PopulateDropDownLists(position);
            return View(position);
        }

        // GET: Positions/Edit/5
        [Authorize(Roles = "Admin, Supervisor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .Include(p=>p.SkillPostions)
                .ThenInclude(p=>p.Skill)
                .AsNoTracking()
                .SingleOrDefaultAsync(p=>p.ID == id);
            if (position == null)
            {
                return NotFound();
            }
            PopulateAssignedSkillData(position);
            PopulateDropDownLists(position);
            return View(position);
        }

        // POST: Positions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Supervisor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        {

            //Get Position to update
            var positionToUpdate = await _context.Positions
                .Include(p=>p.SkillPostions)
                .ThenInclude(p=>p.Skill)
                .SingleOrDefaultAsync(p => p.ID == id);
            if (positionToUpdate == null)
            {
                return NotFound();
            }
            UpdateSkillPositions(selectedOptions, positionToUpdate);

            if (await TryUpdateModelAsync<Position>(positionToUpdate, "",
                p=>p.Name, p=>p.Description, p=>p.Salary, p=>p.OccupationID))
            {
                try
                {
                    
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionExists(positionToUpdate.ID))
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

                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Positions.Name"))
                    {
                        ModelState.AddModelError("Name", "Unable to create new Position. Position Name cannot duplicate.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to create new Position. Try again, and if the problem persists please contact Administrator.");
                    }
                }

            }
            PopulateAssignedSkillData(positionToUpdate);
            PopulateDropDownLists(positionToUpdate);
            return View(positionToUpdate);
        }

        // GET: Positions/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .Include(p => p.Occupation)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            try
            {
                _context.Positions.Remove(position);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {

                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to delete Position. You cannot delete Positon already have Posting.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to delete Position. Try again, and if the problem persists please contact Administrator.");
                }
            }
            return View(position);
        }


        private void PopulateDropDownLists(Position position = null)
        {
            var oQuery = from o in _context.Occupations
                         orderby o.Title
                         select o;
            ViewData["OccupationID"] = new SelectList(oQuery, "ID", "Title", position?.OccupationID);
        }

        private void PopulateAssignedSkillData(Position position)
        {
            var allOptions = _context.Skills;
            var currentOptionsHS = new HashSet<int>(position.SkillPostions.Select(s => s.SkillID));
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach(var s in allOptions)
            {
                if(currentOptionsHS.Contains(s.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.Name
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.Name
                    });
                }
            }
            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");

        }

        private void UpdateSkillPositions (string[] selectedOptions, Position positionToUpdate)
        {
            if(selectedOptions == null)
            {
                positionToUpdate.SkillPostions = new List<SkillPostion>();
                return;
            }
            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(positionToUpdate.SkillPostions.Select(b => b.SkillID));
            foreach(var s in _context.Skills)
            {
                if(selectedOptionsHS.Contains(s.ID.ToString()))
                {
                    if(!currentOptionsHS.Contains(s.ID))
                    {
                        positionToUpdate.SkillPostions.Add(new SkillPostion
                        {
                            SkillID = s.ID,
                            PositionID = positionToUpdate.ID
                        });
                    }
                }
                else
                {
                    if(currentOptionsHS.Contains(s.ID))
                    {
                        SkillPostion specToRemove = positionToUpdate.SkillPostions.SingleOrDefault(p => p.SkillID == s.ID);
                        _context.Remove(specToRemove);
                    }
                }
            }
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.ID == id);
        }
    }
}
