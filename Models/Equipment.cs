using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Inventarisation.Models
{
    public class Equipment
    {
        [Key]
        public Guid? InventNumber { get; set; }

        public string Name { get; set; } = "";

        public byte[]? Image { get; set; }
        public Auditory? Auditory { get; set; }=new Auditory();

        public int? AuditoryId { get; set; }

        public User ResponsibleUser { get; set; }=new User();

        public int? ResponsibleUserId { get; set; }

        public User? TempResponsibleUser { get; set; } = new User();
        public int? TempResponsibleUserId { get; set; }

        public double Price { get; set; }

        public EquipmentStatus? Status { get; set; } = new EquipmentStatus();
        public int StatusId { get; set; }
        public EquipmentModel? EquipmentModel { get; set; } = new EquipmentModel();
        public int EquipmentModelId { get; set; }

        public string? Comment { get; set; }

        public int Count { get; set; } = 0;
        public Direction? Direction { get; set; } = new Direction();
        public int DirectionId { get; set; }



        public List<EquipmentSettings>? EquipmentSettings { get; set; } = new List<EquipmentSettings>();
        public List<Consumables>? Consumables { get; set; } = new List<Consumables>();
        public List<ProgramClass>? Programms { get; set; } = new List<ProgramClass>();
    }
}
