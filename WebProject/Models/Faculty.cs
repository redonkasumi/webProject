using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models
{
    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TelephoneNo { get; set; }
        public string Email { get; set; }

        public Department Department { get; set; }
        [ForeignKey("DepartmentId ")]
        public int DepartmentId { get; set; }

    }
}
