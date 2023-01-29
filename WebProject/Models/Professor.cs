using Microsoft.AspNetCore.Identity;
using System;

using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
    public class Professor 
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public Faculty Faculty { get; set; }
        [ForeignKey("FacultyId ")]
        public int? FacultyId { get; set; }
    }
}

