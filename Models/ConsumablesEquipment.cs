namespace Inventarisation.Models
{
    public class ConsumablesEquipment
    {
        public int Id { get; set; }
        public Consumables Consumable { get; set; }
        public Equipment Equipment { get; set; }
    }
}
