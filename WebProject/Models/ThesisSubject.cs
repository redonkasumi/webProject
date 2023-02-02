using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models
{
    public class ThesisSubject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public DateTime Date { get; set; }
        public Faculty Faculty { get; set; }
        [ForeignKey("FacultyId ")]
        public int? FacultyId { get; set; }

    }
}
