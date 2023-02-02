using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels
{

    public class FacultyEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TelephoneNo { get; set; }
        public string Email { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
    }
}