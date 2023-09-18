namespace Inventarisation.Models
{
    public class Specifications
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public List<ConsumablesType> ConsumablesType { get; set; }
    }
}
