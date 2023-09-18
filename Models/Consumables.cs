using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Inventarisation.Models
{
    public class Consumables
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateOfCame { get; set; }
        public byte[]? Image { get; set; }
        public int Count { get; set; }
        public User ResponsibleUser { get; set; }
        public int ResponsibleUserId { get; set; }
        public User TempResponsibleUser { get; set; }
        public int TempResponsibleUserId { get; set; }
        public ConsumablesType ConsumablesType { get; set; }=new ConsumablesType();
        public int ConsumableTypeId { get; set; }
        public List<Specifications> Specifications { get; set; }=new List<Specifications>();
    }
}
