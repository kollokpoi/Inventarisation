using Inventarisation.Models;
using Inventarisation.ViewModels;

namespace Inventarisation.Interfaces
{
    public interface IBDWork
    {
        public Task<User> Login(string login, string Password);
        public Task<User> GetUser(int id);
        public Task<string> GetUserRole(int id);
        public Task<List<Equipment>> GetShortEquipment();
        public Task<Equipment> GetShortEquipment(Guid id);
        public Task<Equipment> GetFullEquipment(Guid id);
        public Task<List<Auditory>> GetAuditories();
        public Task<Auditory> GetAuditoryInfo(int id);
        public Task<List<ProgramClass>> GetPrograms();
        public Task<List<ProgramClass>> GetEquipmentPrograms(Guid id);
        public Task<List<User>> GetUsers();
        public Task<List<Consumables>> GetConsumables();
        public Task<List<Consumables>> GetConsumablesForEquip(int id);
        public Task<List<Equipment>> GetShortEquipmentByProg(int id);
        public Task<EquipmentAddViewModel> GetEquipmentAddViewModel();
        public Task<List<User>> GetShortUsers();
        public Task<List<Direction>> GetDirections();
        public Task<List<EquipmentStatus>> GetEquipmentStatus();
        public Task<Equipment> GetEquipmentSettings(string ip);
        public void AddEquipment(EquipmentAddViewModel equipmentAddViewModel);
        public void UpdateEquipment(Guid id, int Auditory, int Status, int ResponsibleUserId, int TempResponsibleUserId, string Comment,int Count);
        public void AddEquipmentSettings(Guid id, EquipmentSettings settings);
        public void AddEquipmentProgram(Guid equip, int progId);
        public void AddAuditory(string Name, string ShortName, int ResponsibleUserId, int TempResponsibleUserId);
        public void AddProgram(string Name, string Version, string Creator);
        public Task<List<UserRole>> GetUserRoles();
        public void AddUser(User user);
        public void UpdateAuditory(Auditory aud);
        public Task<ProgramClass> GetProgram(int id);
        public void UpdateProgram(int id,string Name, string Version, string Creator);
        public void UpdateUser(int id,User user);
        public Task<User> GetFullUser(int id);
        public int? AddEquipmentModel(string Name, string Type);
        public void AddDirection(string Name);
        public Task<List<Inventariation>> GetInventariations();
        public Task<Inventariation> GetInventariation(int id, int? userId = null);
        public Task<bool> GetInventariationContain(int inventId, int userId);
        public void AddUserToInventarisation(int inventId, int userId);
        public void UpdateInventarisationEquipment(int id, int count);
        public void AddInventarisation(AddInventarisationViewModel viewModel);
        public Task<Consumables> GetConsumable(int id);
        public Task<List<ConsumablesType>> GetConsumableTypes();
        public Task<List<Specifications>> GetConsumableSpecifications(int id);
        public Task<int> AddConsumableTypeAsync(string Name);
        public Task<int> AddConsumableSpecifications(string Name, int ConsumableId);
        public void AddConsumable(Consumables Consumable, List<ConsumablesSpecificationsValues> values);
        public void UpdateConsumable(int Id, int Count, int ResponsibleUserId, int TempResponsibleUserId);
        public Task<List<StoryItem>> StoryOfMove(Guid Id);
        public Task<List<StoryItem>> StoryOfResponce(Guid Id);
        public void DeleteAuditory(int Id);
        public void DeleteConsumable(int Id);
        public void DeleteEquipment(Guid Id);
        public void DeleteInventarisation(int Id);
        public void DeleteProgram(int Id);
        public void DeleteUser(int Id);
        public void AddEquipTypeConsumables(int Type, List<int> cons);
        public Task<int> GetUserByFullName(string Name, string SecondName, string LastName);
        public Task<int> GetEquipmentTypeByNameAsync(string Name);
        public Task<int> GetEquipmentModel(string Name, int Type);
        public Task<int> GetDirectionId(string Name);
        public void AddImportedEquipmentAsync(string Name, int ResponsibleUserId, int EquipmentModelId,int DirectionId, int Count);
        public Task<List<Equipment>> GetDirectionEquipment(int DirId);
    }
}
