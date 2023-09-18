using Inventarisation.Models;

namespace Inventarisation.ViewModels
{
    public class UserUpdateViewModel
    {
        public User User { get; set; }
        public List<UserRole> UserRoles { get; set; }
    }
}
