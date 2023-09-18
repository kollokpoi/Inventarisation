namespace Inventarisation.Models
{
    public class ConsumablesSpecificationsValues
    {
        public int Id { get; set; } 
        public Specifications? Specification { get; set; } = null;
        public int SpecificationId { get; set; }
        public Consumables? Consumable { get; set; } = null;
        public string Value { get; set; } = "";
    }
}
