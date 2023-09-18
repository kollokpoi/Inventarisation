
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inventarisation.Models
{
    public class Auditory
    {
        public int Id { get; set; } = 0;
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string ShortName { get; set; } = "";
        public User ResponsibleUser { get; set; }=new User();
        [Required]
        public int ResponsibleUserId { get; set; }
        public User? TempResponsibleUser { get; set; }
        public int TempResponsibleUserId { get; set; } = 0;
        public List<Equipment> Equipment { get; set; } = new List<Equipment>();
    }
}
