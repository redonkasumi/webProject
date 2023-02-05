using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebProject.ViewModels.Student
{
    public class StudentCreateViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public IEnumerable<SelectListItem> Faculties { get; set; }
        public int? FacultyId { get; set; }
        public string Thesis { get; set; }
    }
}
