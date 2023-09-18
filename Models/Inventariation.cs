using System;
using System.Collections.Generic;
using System.Text;

namespace Inventarisation.Models
{
    public class Inventariation
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public List<InventarisationEquipment> Equipment { get; set; } = new List<InventarisationEquipment>();
    }
}
