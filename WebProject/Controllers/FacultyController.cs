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
using WebProject.ViewModels;

namespace WebApplication20.Controllers
{
    [Authorize]
    public class FacultyController : Controller
    {
        private readonly AppDBContext _context;

        public FacultyController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Universities
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

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DepartSortParm = sortOrder == "Name" ? "depart_desc" : "Name";
            ViewBag.TelephoneNoSortParm = String.IsNullOrEmpty(sortOrder) ? "tel_desc" : "";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";

            var faculties = from s in _context.Faculties
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                faculties = faculties.Where(s => s.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "depart_desc":
                    faculties = faculties.OrderByDescending(s => s.Department);
                    break;
                case "tel_desc":
                    faculties = faculties.OrderByDescending(s => s.TelephoneNo);
                    break;
                case "email_desc":
                    faculties = faculties.OrderByDescending(s => s.Email);
                    break;
                case "Name":
                    faculties = faculties.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    faculties = faculties.OrderByDescending(s => s.Name);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(faculties.ToPagedList(pageNumber, pageSize));
        }

        // GET: Universities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var university = await _context.Faculties.FirstOrDefaultAsync(m => m.Id == id);

            /* var university = await _context.Faculties.Include(x => x.Students)
                 .FirstOrDefaultAsync(m => m.Id == id);
             if (university == null)
             {
                 return NotFound();
             }
            */

            return View(university);
        }

        // GET: Universities/Create
        public IActionResult Create()
        {
            var departments = _context.Departments.ToList();
            var viewModel = new FacultyCreateViewModel
            {
                Departments = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.DepartmentName
                })
            };
            return View(viewModel);
        }

        // POST: Universities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DepartmentId,TelephoneNo,Email")] Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(faculty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(faculty);
        }

        // GET: Universities/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var faculty = _context.Faculties.Find(id);
            if (faculty == null)
            {
                return NotFound();
            }

            var departments = _context.Departments.ToList();
            var viewModel = new FacultyEditViewModel
            {
                Id = faculty.Id,
                Name = faculty.Name,
                TelephoneNo = faculty.TelephoneNo,
                Email = faculty.Email,
                DepartmentId = faculty.DepartmentId,
                Departments = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.DepartmentName,
                    Selected = d.Id == faculty.DepartmentId
                })
            };
            return View(viewModel);
        }

        // POST: Universities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DepartmentId,TelephoneNo,Email")] Faculty faculty)
        {
            if (id != faculty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faculty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UniversityExists(faculty.Id))
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
            return View(faculty);
        }

        // GET: Universities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await _context.Faculties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (university == null)
            {
                return NotFound();
            }

            return View(university);
        }

        // POST: Universities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var university = await _context.Faculties.FindAsync(id);
            _context.Faculties.Remove(university);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UniversityExists(int id)
        {
            return _context.Faculties.Any(e => e.Id == id);
        }
    }
}
