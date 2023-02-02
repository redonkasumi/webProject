using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.ViewModels.ThesisSubject
{
    public class ThesisSubjectCreateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<SelectListItem> Faculties { get; set; }
        public int? FacultyId { get; set; }
    }
}
