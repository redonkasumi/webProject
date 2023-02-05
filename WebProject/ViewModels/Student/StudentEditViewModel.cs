using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebProject.ViewModels.Student
{
    public class StudentEditViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        [Display(Name = "Faculty")]
        public int? FacultyId { get; set; }
        public IEnumerable<SelectListItem> Faculties { get; set; }
        public string Thesis { get; set; }
    }
}
