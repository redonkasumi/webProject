using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WebProject.Models;

namespace WebProject.ViewModels
{
    public class ProfessorCreateViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public int FacultyId { get; set; }
        public IEnumerable<SelectListItem> Faculties { get; set; }
    }
}