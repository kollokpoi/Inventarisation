using Inventarisation.Models;

namespace Inventarisation.ViewModels
{
    public class AddConsumablesViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<ConsumablesType> ConsumableTypes { get; set; } = new List<ConsumablesType>();
        public List<Specifications> Specifications { get; set; } = new List<Specifications>();
    }
}
