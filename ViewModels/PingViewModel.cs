using Inventarisation.Models;

namespace Inventarisation.ViewModels
{
    public class PingViewModel
    {
        public string Ip { get; set; }
        public string Mac { get; set; }
        public Equipment equipment { get; set; }
    }
}
