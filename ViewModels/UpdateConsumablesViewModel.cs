using Inventarisation.Models;

namespace Inventarisation.ViewModels
{
    public class UpdateConsumablesViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
        public Consumables Consumable { get; set; } = new Consumables();
    }
}
