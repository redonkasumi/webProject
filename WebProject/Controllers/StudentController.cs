using System;
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
using System.Text.Encodings.Web;
using WebProject.ViewModels.Student;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;

namespace WebApplication20.Controllers
{
    [Authorize(Roles = "Professor")]
    public class StudentController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public StudentController(AppDBContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
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

            ViewBag.StudentNameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";

            var students = from s in _context.Students
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.FirstName.Contains(searchString)
                                       || s.FirstName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "last_name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "email_desc":
                    students = students.OrderByDescending(s => s.Email);
                    break;
                case "Name":
                    students = students.OrderBy(s => s.FirstName);
                    break;
                case "name_desc":
                    students = students.OrderByDescending(s => s.FirstName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await _context.Students.FirstOrDefaultAsync(m => m.Id == id);

            return View(student);
        }

        public IActionResult Create()
        {
            var faculties = _context.Faculties.ToList();
            var viewModel = new StudentCreateViewModel
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
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Gender, Address, DateOfBirth, Email, FacultyId, Thesis")] Student student)
        {
            var password = this.GeneratePassword();
            var user = new IdentityUser { UserName = student.Email, Email = student.Email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await this.SendEmail(user.Email, password);

                if (!await _roleManager.RoleExistsAsync("Student"))
                {
                    // Create the "User" role
                    var userRole = new IdentityRole("Student");
                    await _roleManager.CreateAsync(userRole);
                }
                await _userManager.AddToRoleAsync(user, "Student");

            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            if (ModelState.IsValid && result.Succeeded)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var studentModel = new StudentCreateViewModel
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Gender = student.Gender,
                Address = student.Address,
                DateOfBirth = student.DateOfBirth,
                Email = student.Email,
                Thesis = student.Thesis,
                FacultyId = student.FacultyId
            };
            return View(studentModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var faculties = _context.Faculties.ToList();
            var viewModel = new StudentEditViewModel
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Gender = student.Gender,
                Address = student.Address,
                DateOfBirth = student.DateOfBirth,
                Email = student.Email,
                Thesis = student.Thesis,
                FacultyId = student.FacultyId,
                Faculties = faculties.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = d.Id == student.FacultyId
                })
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, FirstName, LastName, Gender, Address, DateOfBirth, Email, FacultyId, Thesis")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
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

