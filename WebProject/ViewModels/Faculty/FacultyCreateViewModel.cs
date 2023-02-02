using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using WebProject.Models;

namespace WebProject.ViewModels
{
    public class FacultyCreateViewModel
    {
        public string Name { get; set; }
        public string TelephoneNo { get; set; }
        public string Email { get; set; }
        public List<Faculty> Faculty { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public int DepartmentId { get; set; }
    }
}