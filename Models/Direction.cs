using System;
using System.Collections.Generic;
using System.Text;

namespace Inventarisation.Models
{
    public class Direction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Equipment> Equipments { get; set; }
    }
}
