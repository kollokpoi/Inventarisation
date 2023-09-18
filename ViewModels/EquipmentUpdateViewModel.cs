using Inventarisation.Models;

namespace Inventarisation.ViewModels
{
    public class EquipmentUpdateViewModel
    {
        public Equipment Equipment { get; set; }

        public List<User>? Users { get; set; } = new List<User> { new User() };
        public List<Auditory>? Auditoryes { get; set; } = new List<Auditory>();
        public List<EquipmentStatus>? Statuses { get; set; } = new List<EquipmentStatus>();
        public List<ProgramClass>? Programs { get; set; }=new List<ProgramClass>();
    }
}
