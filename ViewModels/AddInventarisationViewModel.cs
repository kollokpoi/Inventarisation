namespace Inventarisation.ViewModels
{
    public class AddInventarisationViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Name { get; set; } = "";
        public List<Guid> EquipmentNumbers { get; set; } = new List<Guid>();
    }
}
