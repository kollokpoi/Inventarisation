using Inventarisation.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventarisation.ViewModels
{
    public class EquipmentAddViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public byte[]? Image { get; set; }
        public int? TempResponsibleUserId { get; set; }
        public string? Comment { get; set; }
        public int StatusId { get; set; } = 4;

        [Required]
        public string Name { get; set; } = "";
        [Required]
        public int DirectionId { get; set; }
        [Required]
        public double? Price { get; set; }
        [Required]
        public int AuditoryId { get; set; }
        [Required]
        public int ResponsibleUserId { get; set; }
        [Required]
        public int EquipmentModelId { get; set; }

        public int Count { get; set; } = 0;

        public List<Direction>? Directions { get; set; } = new List<Direction>();
        public List<User>? Users { get; set; } = new List<User> { new User() };
        public List<Auditory>? Auditoryes { get; set; } = new List<Auditory>();
        public List<EquipmentModel>? EquipmentModels { get; set; } = new List<EquipmentModel>();
    }
}
