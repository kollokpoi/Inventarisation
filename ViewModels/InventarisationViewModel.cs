using Inventarisation.Models;

namespace Inventarisation.ViewModels
{
    public class InventarisationViewModel
    {
        public List<Inventariation> Contains { get; set; } = new List<Inventariation>();
        public List<Inventariation> Other { get; set; } = new List<Inventariation>();
    }
}
