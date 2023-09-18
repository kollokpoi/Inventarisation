using System;
using System.Collections.Generic;
using System.Text;

namespace Inventarisation.Models
{
    public class UserInventarisation
    {
        public int Id { get; set; }
        public Equipment Equipment { get; set; }
        public User User { get; set; }
        public string Comment { get; set; }
    }
}
