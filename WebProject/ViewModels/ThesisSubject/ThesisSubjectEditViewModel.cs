using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebProject.ViewModels.ThesisSubject
{
    public class ThesisSubjectEditViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public DateTime Date { get; set; }
        [Display(Name = "Faculty")]
        public int? FacultyId { get; set; }
        public IEnumerable<SelectListItem> Faculties { get; set; }
    }
}
