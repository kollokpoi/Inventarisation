using Inventarisation.Models;

namespace Inventarisation.ViewModels
{
    public class AuditoryUpdateViewModel
    {
        public Auditory Auditory { get; set; }
        public List<User> Users { get; set; }
    }
}
