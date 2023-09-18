using System;
using System.Collections.Generic;
using System.Text;

namespace Inventarisation.Models
{
    public class EquipmentSettings
    {
        public int Id { get; set; }
        public string IPAddr { get; set; }
        public string Mask { get; set; }
        public string Getaway { get; set; }
        public string DNS { get; set; }
    }
}
