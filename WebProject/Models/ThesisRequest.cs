using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models
{
    public class ThesisRequest
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public string Status { get; set; }
        public ThesisSubject ThesisSubject { get; set; }
        [ForeignKey("ThesisSubjectId")]
        public int ThesisSubjectId { get; set; }
        public Student Student { get; set; }
        [ForeignKey("StudentId ")]
        public int StudentId { get; set; }
        public Professor Professor { get; set; }
        [ForeignKey("ProfessorId ")]
        public int ProfessorId { get; set; }
    }
}
