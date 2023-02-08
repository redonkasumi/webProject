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
using System.Net.Mail;
using System.Net;

namespace WebApplication20.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfessorController(AppDBContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _roleManager = roleManager;
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

            ViewBag.ProfessorNameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";

            var professors = from s in _context.Professors
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                professors = professors.Where(s => s.FirstName.Contains(searchString)
                                       || s.FirstName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "last_name_desc":
                    professors = professors.OrderByDescending(s => s.LastName);
                    break;
                case "email_desc":
                    professors = professors.OrderByDescending(s => s.Email);
                    break;
                case "Name":
                    professors = professors.OrderBy(s => s.FirstName);
                    break;
                case "name_desc":
                    professors = professors.OrderByDescending(s => s.FirstName);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(professors.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var professor = await _context.Professors.FirstOrDefaultAsync(m => m.Id == id);

            return View(professor);
        }

        public IActionResult Create()
        {
            var faculties = _context.Faculties.ToList();
            var viewModel = new ProfessorCreateViewModel
            {
                Faculties = faculties.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Gender, Address, DateOfBirth, Email, FacultyId")] Professor professor)
        {
            var password = this.GeneratePassword();
            var user = new IdentityUser { UserName = professor.Email, Email = professor.Email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await this.SendEmail(user.Email, password);

                if (!await _roleManager.RoleExistsAsync("Professor"))
                {
                    // Create the "User" role
                    var userRole = new IdentityRole("Professor");
                    await _roleManager.CreateAsync(userRole);
                }
                await _userManager.AddToRoleAsync(user, "Professor");

            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            if (ModelState.IsValid && result.Succeeded)
            {
                _context.Add(professor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var professorModel = new ProfessorCreateViewModel
            {
                Id = professor.Id,
                FirstName = professor.FirstName,
                LastName = professor.LastName,
                Gender = professor.Gender,
                Address = professor.Address,
                DateOfBirth = professor.DateOfBirth,
                Email = professor.Email,
                FacultyId = professor.FacultyId
            };
            return View(professorModel);

        }

        public async Task<IActionResult> Edit(int? id)
        {
            var professor = _context.Professors.Find(id);
            if (professor == null)
            {
                return NotFound();
            }

            var faculties = _context.Faculties.ToList();
            var viewModel = new ProfessorEditViewModel
            {
                Id = professor.Id,
                FirstName = professor.FirstName,
                LastName = professor.LastName,
                Gender = professor.Gender,
                Email = professor.Email,
                Address = professor.Address,
                DateOfBirth = professor.DateOfBirth,
                FacultyId = professor.FacultyId,
                Faculties = faculties.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = d.Id == professor.FacultyId
                })
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, FirstName, LastName, Gender, Address, DateOfBirth, Email, FacultyId")] Professor professor)
        {
            if (id != professor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(professor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfessorExists(professor.Id))
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
            return View(professor);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var professor = await _context.Professors.FindAsync(id);
            _context.Professors.Remove(professor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfessorExists(int id)
        {
            return _context.Professors.Any(e => e.Id == id);
        }
        private async Task SendEmail(string email, string password)
        {
            var fromAddress = new MailAddress("smiss.ubt@gmail.com", "smail");
            var toAddress = new MailAddress(email, "Student");
            const string fromPassword = "qgeycmoujfynteai";
            const string subject = "Your Account Information";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = $"Your account has been created. Your email is {email} and password is {password}"
            })
            {
                smtp.Send(message);
            }
        }

        private string GeneratePassword()
        {
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numericChars = "0123456789";
            const string symbolChars = "!@#$%^&*()_+-=[]{}|;':\",.<>/?`~";
            const int passwordLength = 10;

            var password = new char[passwordLength];
            var random = new Random();

            password[0] = lowercaseChars[random.Next(0, lowercaseChars.Length - 1)];
            password[1] = uppercaseChars[random.Next(0, uppercaseChars.Length - 1)];
            password[2] = numericChars[random.Next(0, numericChars.Length - 1)];
            password[3] = symbolChars[random.Next(0, symbolChars.Length - 1)];

            var chars = lowercaseChars + uppercaseChars + numericChars + symbolChars;

            for (int i = 4; i < passwordLength; i++)
            {
                password[i] = chars[random.Next(0, chars.Length - 1)];
            }

            return new string(password);
        }

    }
}

