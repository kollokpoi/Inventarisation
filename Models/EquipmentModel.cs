using System;
using System.Collections.Generic;
using System.Text;

namespace Inventarisation.Models
{
    public class EquipmentModel
    {
        public int Id { get; set; }
        public EquipmentType EquipmentType { get; set; } = new EquipmentType();
        public string Name { get; set; } = "";
        public int EquipmentTypeId { get; set; }
    }
}
