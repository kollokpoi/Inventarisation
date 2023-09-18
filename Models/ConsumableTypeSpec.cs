namespace Inventarisation.Models
{
    public class ConsumableTypeSpec
    {
        public int Id { get; set; }
        public ConsumablesType Type { get; set; }
        public Specifications Specification { get; set; }
    }
}
