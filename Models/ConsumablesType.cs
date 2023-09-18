using System;
using System.Collections.Generic;
using System.Text;

namespace Inventarisation.Models
{
    public class ConsumablesType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Specifications> Specifications { get; set; }
    }
}
