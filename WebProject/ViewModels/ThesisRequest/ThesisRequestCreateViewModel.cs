using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WebProject.Models;

namespace WebProject.ViewModels
{
    public class ThesisRequestCreateViewModel
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public string Status { get; set; }

        public int ThesisSubjectId { get; set; }
        public IEnumerable<SelectListItem> ThesisSubjects { get; set; }

        public int StudentId { get; set; }
        public int ProfessorId { get; set; }
        public IEnumerable<SelectListItem> Professors { get; set; }
        public IEnumerable<SelectListItem> StatusOptions { get; set; }

    }
}