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

namespace WebApplication20.Controllers
{
    [Authorize]
    public class ThesisRequestController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ThesisRequestController(AppDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var student = await _context.Students.FirstOrDefaultAsync(m => m.Email == user.Email);
            var professor = await _context.Professors.FirstOrDefaultAsync(m => m.Email == user.Email);

            var isStudent = await _userManager.IsInRoleAsync(user, "Student");


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

            ViewBag.TopicSortParm = String.IsNullOrEmpty(sortOrder) ? "topic_desc" : "";
            ViewBag.StatusSortParm = String.IsNullOrEmpty(sortOrder) ? "status_desc" : "";
            IEnumerable<ThesisRequest> thesisRequests = null;
            if (isStudent)
            {
                thesisRequests = _context.ThesisRequests.Where(x => x.StudentId == student.Id).ToList();
            }
            else
            {
                thesisRequests = _context.ThesisRequests.Where(x => x.ProfessorId == professor.Id).ToList();
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                thesisRequests = thesisRequests.Where(s => s.Topic.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "topic_desc":
                    thesisRequests = thesisRequests.OrderByDescending(s => s.Topic);
                    break;
                case "status_desc":
                    thesisRequests = thesisRequests.OrderByDescending(s => s.Status);
                    break;
            }

            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(thesisRequests.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var professor = await _context.ThesisRequests.FirstOrDefaultAsync(m => m.Id == id);

            return View(professor);
        }

        public IActionResult Create()
        {
            var thesisSubjects = _context.ThesisSubjects.ToList();
            var professors = _context.Professors.ToList();

            var viewModel = new ThesisRequestCreateViewModel
            {
                Professors = professors.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.FirstName+' '+d.LastName
                }),

                ThesisSubjects = thesisSubjects.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Title
                })
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Topic,Status, ThesisSubjectId, ProfessorId")] ThesisRequest thesisRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            var student = await _context.Students.FirstOrDefaultAsync(m => m.Email == user.Email);
            thesisRequest.StudentId = student.Id;
            if (ModelState.IsValid)
            {
                _context.Add(thesisRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thesisRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var request = _context.ThesisRequests.Find(id);
            if (request == null)
            {
                return NotFound();
            }

            var thesisSubjects = _context.ThesisSubjects.ToList();
            var professors = _context.Professors.ToList();
            var viewModel = new ThesisRequestEditViewModel
            {
                Id = request.Id,
                Topic= request.Topic,
                Status = request.Status,
                ProfessorId =request.ProfessorId,
                ThesisSubjectId=request.ThesisSubjectId,
                Professors = professors.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.FirstName + ' ' + d.LastName,
                    Selected = d.Id == request.ProfessorId
                }),

                ThesisSubjects = thesisSubjects.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Title,
                    Selected = d.Id == request.ThesisSubjectId

                })
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Topic,Status, ThesisSubjectId, ProfessorId")] ThesisRequest thesisRequest)
        {
            if (id != thesisRequest.Id)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            var student = await _context.Students.FirstOrDefaultAsync(m => m.Email == user.Email);
            thesisRequest.StudentId = student.Id;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thesisRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(thesisRequest.Id))
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
            return View(thesisRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.ThesisRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.ThesisRequests.FindAsync(id);
            _context.ThesisRequests.Remove(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestExists(int id)
        {
            return _context.ThesisRequests.Any(e => e.Id == id);
        }
    }
}

