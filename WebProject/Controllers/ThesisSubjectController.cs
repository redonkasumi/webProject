using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProject.Models;
using WebProject.Models.AppDBContext;
using PagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebProject.ViewModels;
using WebProject.ViewModels.ThesisSubject;

namespace WebApplication20.Controllers
{
    [Authorize]
    public class ThesisSubjectController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ThesisSubjectController(AppDBContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            ViewBag.ThesisSubjectNameSortParam = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";

            var thesisSubjects = from s in _context.ThesisSubjects
                             select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                thesisSubjects = thesisSubjects.Where(s => s.Title.Contains(searchString)
                                       || s.Title.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Title":
                    thesisSubjects = thesisSubjects.OrderBy(s => s.Title);
                    break;
                case "title_desc":
                    thesisSubjects = thesisSubjects.OrderByDescending(s => s.Title);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(thesisSubjects.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var thesisSubject = await _context.ThesisSubjects.FirstOrDefaultAsync(m => m.Id == id);

            return View(thesisSubject);
        }
        [Authorize(Roles = "Professor")]

        public IActionResult Create()
        {
            var faculties = _context.Faculties.ToList();
            var viewModel = new ThesisSubjectCreateViewModel
            {
                Faculties = faculties.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Professor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Keywords, Date, FacultyId")] ThesisSubject thesisSubject)
        {

            if (ModelState.IsValid)
            {
                _context.Add(thesisSubject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var thesisSubjectModel = new ThesisSubjectCreateViewModel
            {
                Id = thesisSubject.Id,
                Title = thesisSubject.Title,
                Description = thesisSubject.Description,
                Keywords = thesisSubject.Keywords,
                Date = thesisSubject.Date,
                FacultyId = thesisSubject.FacultyId
            };
            return View(thesisSubjectModel);

        }

        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> Edit(int? id)
        {
            var thesisSubject = _context.ThesisSubjects.Find(id);
            if (thesisSubject == null)
            {
                return NotFound();
            }

            var faculties = _context.Faculties.ToList();
            var viewModel = new ThesisSubjectEditViewModel
            {
                Id = thesisSubject.Id,
                Title = thesisSubject.Title,
                Description = thesisSubject.Description,
                Keywords = thesisSubject.Keywords,
                Date = thesisSubject.Date,
                FacultyId = thesisSubject.FacultyId,
                Faculties = faculties.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = d.Id == thesisSubject.FacultyId
                })
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Professor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Keywords, Date, FacultyId")] ThesisSubject thesisSubject)
        {
            if (id != thesisSubject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thesisSubject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThesisSubjectExists(thesisSubject.Id))
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
            return View(thesisSubject);
        }

        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thesisSubject = await _context.ThesisSubjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thesisSubject == null)
            {
                return NotFound();
            }

            return View(thesisSubject);
        }

        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thesisSubject = await _context.ThesisSubjects.FindAsync(id);
            _context.ThesisSubjects.Remove(thesisSubject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThesisSubjectExists(int id)
        {
            return _context.ThesisSubjects.Any(e => e.Id == id);
        }
    }
}

