namespace Inventarisation.Models
{
    public class InventarisationEquipment : Equipment
    {
        public InventarisationEquipment(Equipment equipment) 
        {
            InventNumber = equipment.InventNumber;
            Name = equipment.Name;
            Image = equipment.Image;
            AuditoryId = equipment.AuditoryId;
            ResponsibleUserId = equipment.ResponsibleUserId;
            TempResponsibleUserId = equipment.TempResponsibleUserId;
            Price = equipment.Price;
            StatusId = equipment.StatusId;
            EquipmentModelId = equipment.EquipmentModelId;
            Comment = equipment.Comment;
            DirectionId = equipment.DirectionId;
        }
        public int Count { get; set; }
        public bool Checked { get; set; }
        public int InventEquipId { get; set; }
    }
}
