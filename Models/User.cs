using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inventarisation.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Password { get; set; } = "";
        public string? Login { get; set; } = "";
        public UserRole UserRole { get; set; } = new UserRole();
        public int UserRoleId { get; set; }
        public string? Email { get; set; }
        public string? SecondName { get; set; }
        public string? Name { get; set; } = "";
        public string? LastName { get; set; } = "";
        public string? Phone { get; set; } = "";
        public string? Adres { get; set; } = "";

        public string GetFullName()
        {
            return SecondName + " " + Name + " " + LastName;
        }
    }
}
