using Inventarisation.Interfaces;
using Inventarisation.Models;
using Inventarisation.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using NuGet.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Inventarisation.Services
{
    public class BDWork : IBDWork
    {
        private static string connectionString = "server=192.168.1.6;Database=InventarisationDB; User Id=diplomUser; Password=12332155; Trusted_Connection=false; MultipleActiveResultSets=True;TrustServerCertificate=True;";
        private static string secondConnectionString = @"server=DESKTOP-I2HPGD3\SQL;Database=InventarisationDB; User Id=sa; Password=12332155; Trusted_Connection=false; MultipleActiveResultSets=True;TrustServerCertificate=True;";

        private SqlConnection GetConnection()
        {
            try
            {
                SqlConnection connection= new SqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch
            {
                try
                {
                    SqlConnection connection = new SqlConnection(secondConnectionString);
                    connection.Open();
                    return connection;
                }
                catch
                {

                    throw;
                }
            }
            
        }
        public async Task<User> GetUser(int id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT [Id],[UserRoleId],[Email],[SecondName],[Name],[LastName],[Phone],[Adres] FROM [InventarisationDB].[dbo].[User] where Id = {id} and PseudoDelited = 0 ";

            var reader = await command.ExecuteReaderAsync();
            reader.Read();
            if (reader.HasRows)
            {
                User user = new User()
                {
                    Id = reader.GetInt32(0),
                    UserRoleId = reader.GetInt32(1),
                    Email= reader.GetString(2),
                    SecondName= reader.GetString(3),
                    Name= reader.GetString(4),
                    LastName= reader.GetString(5),
                    Phone = reader.GetString(6),
                    Adres= reader.GetString(7),
                    
                };
                return user;
            }
            return null;
        }

        public async Task<User> Login(string login, string Password)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select u.Id,r.Id from [User] as u, UserRole as r where r.Id = u.UserRoleId and u.Login = '{login}' and u.Password = '{Password}' and PseudoDelited=0";

            var reader = await command.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                reader.Read();
                User user = new User()
                {
                    Id = reader.GetInt32(0),
                    UserRoleId = reader.GetInt32(1)
                };
                return user;
            }
            return null;
        }
        public async Task<string> GetUserRole(int id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select [Name] from UserRole where Id = {id}";

            var reader = await command.ExecuteReaderAsync();
            reader.Read();
            return reader.GetString(0);
        }
        public async Task<List<UserRole>> GetUserRoles()
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select * from UserRole";
            List<UserRole> userRoles = new List<UserRole>();
            var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                UserRole role = new()
                {
                    Id= reader.GetInt32(0),
                    Name = reader.GetString(1),
                };
               userRoles.Add(role);
            }
            return userRoles;
        }
        public async Task<List<Equipment>> GetShortEquipment()
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = "select eq.[InventNumber],eq.[Name],eq.[Image],eq.[Price],eq.[Comment],eq.AuditoryId,eq.[Count] FROM [InventarisationDB].[dbo].[Equipment] as eq where eq.PseudoDelete = 0";
            var reader = await command.ExecuteReaderAsync();
            List<Equipment> list = new List<Equipment>();
            while (reader.Read())
            {
                Equipment equipment = new Equipment();
                Auditory auditory = new Auditory();

                equipment.InventNumber = reader.GetGuid(0);
                equipment.Name= reader.GetString(1);
                equipment.Image = reader.IsDBNull(2)? null: (byte[])reader.GetValue(2);
                equipment.Price = reader.GetDouble(3);
                equipment.Comment = reader.IsDBNull(4) ? null : reader.GetString(4);
                equipment.Count = reader.GetInt32(6);

                if (!reader.IsDBNull(5))
                {
                    var secondCommand = connection.CreateCommand();
                    secondCommand.CommandText = $"select [Name] from Auditory where Id = {reader.GetInt32(5)}";
                    var secondReader = await secondCommand.ExecuteReaderAsync();
                    secondReader.Read();
                    auditory.Id = reader.GetInt32(5);
                    auditory.Name = secondReader.GetString(0);
                    equipment.Auditory = auditory;
                }


                list.Add(equipment);
            }

            
            return list;
        }
        public async Task<Equipment> GetFullEquipment(Guid id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select * FROM [InventarisationDB].[dbo].[Equipment] where InventNumber='{id}' and PseudoDelete = 0";
            var reader = await command.ExecuteReaderAsync();

            reader.Read();
            Equipment equipment = new()
            {
                InventNumber = reader.GetGuid(0),
                Name = reader.GetString(1),
                Image = reader.IsDBNull(2) ? null : (byte[])reader.GetValue(2),
                ResponsibleUserId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                TempResponsibleUserId= reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                AuditoryId = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                Price=(double)reader.GetDouble(6),
                Comment= reader.IsDBNull(9) ? null : reader.GetString(9),
                DirectionId =reader.GetInt32(10),
                EquipmentModelId= reader.GetInt32(8),
                StatusId=reader.GetInt32(7),
                Count=reader.GetInt32(12),
            };
            equipment.Status.Id= reader.GetInt32(7);
            equipment.EquipmentModel.Id= reader.GetInt32(8);

            equipment.Direction.Id= reader.GetInt32(10);

            reader.Close();
            if (equipment.ResponsibleUserId is not null)
            {
                command.CommandText = $"SELECT [Id],[SecondName],[Name],[LastName]  FROM [InventarisationDB].[dbo].[User] where Id={equipment.ResponsibleUserId}";
                reader = await command.ExecuteReaderAsync();

                reader.Read();
                equipment.ResponsibleUser.Id = reader.GetInt32(0);
                equipment.ResponsibleUser.SecondName = reader.GetString(1);
                equipment.ResponsibleUser.Name = reader.GetString(2);
                equipment.ResponsibleUser.LastName = reader.GetString(3);

                reader.Close();
            }


            if (equipment.TempResponsibleUserId!=0)
            {
                command.CommandText = $"SELECT [Id],[SecondName],[Name],[LastName]  FROM [InventarisationDB].[dbo].[User] where Id={equipment.ResponsibleUserId}";
                reader = await command.ExecuteReaderAsync();

                reader.Read();

                equipment.TempResponsibleUser.Id= reader.GetInt32(0);
                equipment.TempResponsibleUser.SecondName = reader.GetString(1);
                equipment.TempResponsibleUser.Name= reader.GetString(2);
                equipment.TempResponsibleUser.LastName= reader.GetString(3);
                reader.Close();
            }

            command.CommandText = $"select * from EquipmentStatus where Id = {equipment.Status.Id}";
            reader = await command.ExecuteReaderAsync();

            reader.Read();
            equipment.Status.Name= reader.GetString(1);
            reader.Close();

            command.CommandText = $"select * from EquipmentModel where Id = {equipment.EquipmentModel.Id}";
            reader = await command.ExecuteReaderAsync();

            reader.Read();
            equipment.EquipmentModel.Name = reader.GetString(1);
            equipment.EquipmentModel.EquipmentTypeId = reader.GetInt32(2);
            reader.Close();

            command.CommandText = $"select * from EquipmentType where Id = {equipment.EquipmentModel.EquipmentTypeId}";
            reader = await command.ExecuteReaderAsync();

            reader.Read();
            equipment.EquipmentModel.EquipmentType.Id = reader.GetInt32(0);
            equipment.EquipmentModel.EquipmentType.Name = reader.GetString(1);
            reader.Close();

            command.CommandText = $"select * from EquipmentSettings where EquipmentId = '{equipment.InventNumber}'";
            reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                EquipmentSettings equipmentSettings = new()
                {
                    Id = reader.GetInt32(0),
                    IPAddr = reader.GetString(1),
                    Mask = reader.GetString(2),
                    Getaway = reader.GetString(3),
                    DNS = reader.GetString(4)
                };
                equipment.EquipmentSettings.Add(equipmentSettings);
            }

            reader.Close();

            command.CommandText = $"select * from Direction where Id = {equipment.Direction.Id}";
            reader = await command.ExecuteReaderAsync();

            reader.Read();
            equipment.Direction.Id = reader.GetInt32(0);
            equipment.Direction.Name = reader.GetString(1);
            reader.Close();

            equipment.Consumables = await GetConsumablesForEquip(equipment.EquipmentModel.EquipmentTypeId);
            if (equipment.AuditoryId!=null)
            {
                equipment.Auditory = await GetAuditoryInfo((int)equipment.AuditoryId);
            }
            
            equipment.Programms = await GetEquipmentPrograms(id);

            return equipment;
        }
        public async Task<List<Auditory>> GetAuditories()
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select * FROM [InventarisationDB].[dbo].[Auditory]";
            var reader = await command.ExecuteReaderAsync();

            List<Auditory> list = new List<Auditory>();

            while (reader.Read())
            {
                Auditory auditory = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    ShortName = reader.GetString(2),
                    ResponsibleUserId = reader.GetInt32(3),
                    TempResponsibleUserId = reader.IsDBNull(4)?0:reader.GetInt32(4)
                };

                var userCommand = connection.CreateCommand();
                userCommand.CommandText = $"select [SecondName],[Name],[LastName],[Phone],[Adres] from [User] where Id = {auditory.ResponsibleUserId}";

                var userReader = await userCommand.ExecuteReaderAsync();
                userReader.Read();

                User responsoibleUser = new()
                {
                    SecondName = userReader.GetString(0),
                    Name = userReader.GetString(1),
                    LastName = userReader.GetString(2),
                };
                auditory.ResponsibleUser = responsoibleUser;

                userReader.Close();

                if (auditory.TempResponsibleUserId !=0)
                {
                    userCommand.CommandText = $"select [SecondName],[Name],[LastName],[Phone],[Adres] from [User] where Id = {auditory.ResponsibleUserId}";

                    userReader = await userCommand.ExecuteReaderAsync();
                    userReader.Read();

                    User tempResponsoibleUser = new()
                    {
                        SecondName = userReader.GetString(0),
                        Name = userReader.GetString(1),
                        LastName = userReader.GetString(2),
                    };
                    auditory.TempResponsibleUser = tempResponsoibleUser;
                    userReader.Close();
                }

                list.Add(auditory);
            }
            return list;
        }
        public async Task<Auditory> GetAuditoryInfo(int id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select * FROM [InventarisationDB].[dbo].[Auditory] where Id = {id}";
            var reader = await command.ExecuteReaderAsync();

            reader.Read();
            Auditory auditory = new()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                ShortName = reader.GetString(2),
                ResponsibleUserId = reader.GetInt32(3),
                TempResponsibleUserId = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
            };

            reader.Close();

            command.CommandText = $"select [SecondName],[Name],[LastName],[Phone],[Adres] from [User] where Id = {auditory.ResponsibleUserId}";

            reader = await command.ExecuteReaderAsync();
            reader.Read();

            User responsoibleUser = new()
            {
                SecondName = reader.GetString(0),
                Name = reader.GetString(1),
                LastName = reader.GetString(2),
            };
            auditory.ResponsibleUser = responsoibleUser;

            reader.Close();

            if (auditory.TempResponsibleUserId != 0)
            {
                command.CommandText = $"select [SecondName],[Name],[LastName],[Phone],[Adres] from [User] where Id = {auditory.ResponsibleUserId}";

                reader = await command.ExecuteReaderAsync();
                reader.Read();

                User tempResponsoibleUser = new()
                {
                    SecondName = reader.GetString(0),
                    Name = reader.GetString(1),
                    LastName = reader.GetString(2),
                };
                auditory.TempResponsibleUser = tempResponsoibleUser;
                reader.Close();
            }

            command.CommandText = $"select InventNumber from Equipment where AuditoryId = {auditory.Id} and PseudoDelete = 0";
            reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                Equipment equipment = await GetShortEquipment(reader.GetGuid(0));
                auditory.Equipment.Add(equipment);
            }

            return auditory;
        }
        public async Task<Equipment> GetShortEquipment(Guid id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select eq.[InventNumber],eq.[Name],eq.[Image], eq.[Price], eq.[Comment],eq.[Count] FROM [InventarisationDB].[dbo].[Equipment] as eq where eq.InventNumber='{id}' and eq.PseudoDelete = 0";
            var reader = await command.ExecuteReaderAsync();
            reader.Read();

            Equipment equipment = new Equipment();

            equipment.InventNumber = reader.GetGuid(0);
            equipment.Name = reader.GetString(1);
            equipment.Image = reader.IsDBNull(2) ? null : (byte[])reader.GetValue(2);
            equipment.Price = reader.GetDouble(3);
            equipment.Comment = reader.GetString(4);
            equipment.Count = reader.GetInt32(5);

            return equipment;
        }
        public async Task<List<ProgramClass>> GetPrograms()
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select p.[Id],p.Name,p.[Version],pc.Id,pc.[Name]  FROM Programms as p, ProgramCreators as pc where p.Creators = pc.Id";
            var reader = await command.ExecuteReaderAsync();

            List<ProgramClass> programs = new List<ProgramClass>();
            while (reader.Read())
            {
                ProgramClass programClass = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Version = reader.GetString(2),

                };
                ProgramCreator creator = new()
                {
                    Id = reader.GetInt32(3),
                    Name = reader.GetString(4),
                };

                programClass.ProgramCreator = creator;
                programs.Add(programClass);
            }

            return programs;
        }
        public async Task<List<ProgramClass>> GetEquipmentPrograms(Guid id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select p.[Id],p.Name,p.[Version],pc.Id,pc.[Name]  FROM Programms as p, ProgramCreators as pc where p.Creators = pc.Id and p.Id in(select Program from EquipmentProgram where Equipment = '{id}')";
            var reader = await command.ExecuteReaderAsync();

            List<ProgramClass> programs = new List<ProgramClass>();
            while (reader.Read())
            {
                ProgramClass programClass = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Version = reader.GetString(2),

                };
                ProgramCreator creator = new()
                {
                    Id = reader.GetInt32(3),
                    Name = reader.GetString(4),
                };

                programClass.ProgramCreator = creator;
                programs.Add(programClass);
            }

            return programs;
        }
        public async Task<List<User>> GetUsers()
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select * from [User] where PseudoDelited = 0";
            var reader = await command.ExecuteReaderAsync();

            List <User> users = new List<User>();
            while (reader.Read())
            {
                User user = new()
                {
                    Id= reader.GetInt32(0),
                    Password = reader.GetString(1),
                    Login= reader.GetString(2),
                    UserRoleId= reader.GetInt32(3),
                    Email= reader.IsDBNull(4) ? null : reader.GetString(4),
                    SecondName = reader.GetString(5),
                    Name= reader.GetString(6),
                    LastName= reader.GetString(7),
                    Phone= reader.IsDBNull(8) ? null : reader.GetString(8),
                    Adres = reader.IsDBNull(9) ? null : reader.GetString(9),
                };

                var userCommand = connection.CreateCommand();
                userCommand.CommandText = $"select * from [UserRole] where id = {user.UserRoleId}";
                var userReader = await userCommand.ExecuteReaderAsync();
                userReader.Read();
                user.UserRole.Name = userReader.GetString(1);

                users.Add(user);
            }

            return users;
        }
        public async Task<List<User>> GetShortUsers()
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select [Id],[SecondName],[Name],[LastName] from [User] where PseudoDelited = 0";
            var reader = await command.ExecuteReaderAsync();

            List<User> users = new List<User>();
            while (reader.Read())
            {
                User user = new()
                {
                    Id = reader.GetInt32(0),
                    SecondName = reader.GetString(1),
                    Name = reader.GetString(2),
                    LastName = reader.GetString(3),
                };
                users.Add(user);
            }

            return users;
        }
        public async Task<List<Consumables>> GetConsumables()
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select c.[Id],c.[Name],c.[Description],c.[DateOfCame],c.[Image],c.[Count],c.[ResponsibleUserId],c.[TempResponsibleUserId],c.[ConsumablesTypeId], ct.[name] " +
                $"from [Consumables] as c, [ConsumablesType] as ct where c.[ConsumablesTypeId] = ct.Id";
            var reader = await command.ExecuteReaderAsync();

            List<Consumables> consumables= new List<Consumables>();

            while (reader.Read())
            {
                Consumables consumable = new()
                {
                    Id= reader.GetInt32(0),
                    Name= reader.GetString(1),
                    Description= reader.GetString(2),
                    DateOfCame = reader.GetDateTime(3),
                    Image = reader.IsDBNull(4)? null: (byte[]) reader.GetValue(4),
                    Count= reader.GetInt32(5),
                    ResponsibleUserId= reader.GetInt32(6),
                    TempResponsibleUserId = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                };
                consumable.ConsumablesType.Id= reader.GetInt32(8);
                consumable.ConsumablesType.Name= reader.GetString(9);



                var secondCommand = connection.CreateCommand();
                secondCommand.CommandText = $"select s.Id,s.[Name], csv.[Value] " +
                    $"from [ConsumablesTypeSpecifications] as cts, Specifications as s,ConsumablesSpecificationsValues as csv " +
                    $"where cts.ConsumablesTypeId = {consumable.ConsumablesType.Id} and s.Id = cts.SpecificationsId and csv.SpecificationId = s.Id and csv.ConsumableId = {consumable.Id}";
                var secondReader = await secondCommand.ExecuteReaderAsync();

                while (secondReader.Read())
                {
                    Specifications specifications = new()
                    {
                        Id= secondReader.GetInt32(0),
                        Name= secondReader.GetString(1),
                        Value= secondReader.GetString(2),
                    };

                    consumable.Specifications.Add(specifications);
                }

                secondReader.Close();

                consumable.ResponsibleUser = await GetUser(consumable.ResponsibleUserId);

                if (consumable.TempResponsibleUserId!=0)
                {
                    consumable.TempResponsibleUser = await GetUser(consumable.TempResponsibleUserId);
                }

                consumables.Add(consumable);
            }

            return consumables;
        }
        public async Task<List<Consumables>> GetConsumablesForEquip(int id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select c.[Id],c.[Name],c.[Description],c.[DateOfCame],c.[Image],c.[Count],c.[ResponsibleUserId],c.[TempResponsibleUserId],c.[ConsumablesTypeId], ct.[name] " +
                $"from [Consumables] as c, [ConsumablesType] as ct where c.[ConsumablesTypeId] = ct.Id and c.Id in(select Consumable from EquipmentConsumables where EquipmentType = {id}) ";
            var reader = await command.ExecuteReaderAsync();

            List<Consumables> consumables = new List<Consumables>();

            while (reader.Read())
            {
                Consumables consumable = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    DateOfCame = reader.GetDateTime(3),
                    Image = reader.IsDBNull(4) ? null : (byte[])reader.GetValue(4),
                    Count = reader.GetInt32(5),
                    ResponsibleUserId = reader.GetInt32(6),
                    TempResponsibleUserId = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                };
                consumable.ConsumablesType.Id = reader.GetInt32(8);
                consumable.ConsumablesType.Name = reader.GetString(9);



                var secondCommand = connection.CreateCommand();
                secondCommand.CommandText = $"select s.Id,s.[Name], csv.[Value] " +
                    $"from [ConsumablesTypeSpecifications] as cts, Specifications as s,ConsumablesSpecificationsValues as csv " +
                    $"where cts.ConsumablesTypeId = {consumable.ConsumablesType.Id} and s.Id = cts.SpecificationsId and csv.SpecificationId = s.Id and csv.ConsumableId = {consumable.Id}";
                var secondReader = await secondCommand.ExecuteReaderAsync();

                while (secondReader.Read())
                {
                    Specifications specifications = new()
                    {
                        Id = secondReader.GetInt32(0),
                        Name = secondReader.GetString(1),
                        Value = secondReader.GetString(2),
                    };

                    consumable.Specifications.Add(specifications);
                }

                secondReader.Close();

                consumable.ResponsibleUser = await GetUser(consumable.ResponsibleUserId);

                if (consumable.TempResponsibleUserId != 0)
                {
                    consumable.TempResponsibleUser = await GetUser(consumable.TempResponsibleUserId);
                }

                consumables.Add(consumable);
            }

            return consumables;
        }
        public async Task<List<Equipment>> GetShortEquipmentByProg(int id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select eq.[InventNumber],eq.[Name],eq.[Image],eq.[Price],eq.[Comment],a.Id,a.Name FROM [InventarisationDB].[dbo].[Equipment] as eq, [InventarisationDB].[dbo].[Auditory] as a where eq.AuditoryId = a.Id and eq.PseudoDelete = 0 and eq.InventNumber in (select Equipment from EquipmentProgram where Program = {id})";
            var reader = await command.ExecuteReaderAsync();
            List<Equipment> list = new List<Equipment>();
            while (reader.Read())
            {
                Equipment equipment = new Equipment();
                Auditory auditory = new Auditory();

                equipment.InventNumber = reader.GetGuid(0);
                equipment.Name = reader.GetString(1);
                equipment.Image = reader.IsDBNull(2) ? null : (byte[])reader.GetValue(2);
                equipment.Price = reader.GetDouble(3);
                equipment.Comment = reader.GetString(4);

                auditory.Id = reader.GetInt32(5);
                auditory.Name = reader.GetString(6);

                equipment.Auditory = auditory;

                list.Add(equipment);
            }
            return list;
        }
        public async Task<EquipmentAddViewModel> GetEquipmentAddViewModel()
        {
            EquipmentAddViewModel equipmentAddViewModelviewModel = new EquipmentAddViewModel();
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = "select * from EquipmentModel";
            var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                equipmentAddViewModelviewModel.EquipmentModels.Add(new EquipmentModel()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    EquipmentTypeId = reader.GetInt32(2),
                });
            }

            equipmentAddViewModelviewModel.Auditoryes = await GetAuditories();
            equipmentAddViewModelviewModel.Users= await GetShortUsers();
            equipmentAddViewModelviewModel.Directions= await GetDirections();

            return equipmentAddViewModelviewModel;
        }
        public async Task<List<Direction>> GetDirections()
        {
            List <Direction> directions = new List <Direction>();
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select * from [Direction] EquipmentProgram";
            var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                Direction direction = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                };
                directions.Add(direction);
            }

            return directions;
        }
        public void AddEquipment(EquipmentAddViewModel eq)
        {

            Guid guid= Guid.NewGuid();
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"insert into [dbo].[Equipment]([InventNumber],[Name]{(eq.Image!=null?",[Image]":"")},[ResponsibleUserId]" +
                $",[TempResponsibleUserId],[AuditoryId],[Price]" +
                $",[StatusId],[EquipmentModelId],[Comment],[DirectionId],[Count])" +
                $" values('{guid}','{eq.Name}'{(eq.Image != null ? ",@Image" : "")},{eq.ResponsibleUserId},{(eq.TempResponsibleUserId==0?"null":eq.TempResponsibleUserId.ToString())}" +
                $",{eq.AuditoryId},{eq.Price},4,{eq.EquipmentModelId},'{eq.Comment}',{eq.DirectionId},{eq.Count})";

            if (eq.Image!=null)
            {
                command.Parameters.Add("@Image", SqlDbType.Image);
                command.Parameters["@Image"].Value = eq.Image;
            }
            command.ExecuteNonQuery();
        }
        public async Task<List<EquipmentStatus>> GetEquipmentStatus()
        {
            List<EquipmentStatus> Statuses = new List<EquipmentStatus>();
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select * from [EquipmentStatus]";
            var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                EquipmentStatus direction = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                };
                Statuses.Add(direction);
            }

            return Statuses;
        }
        public void AddEquipmentProgram(Guid equip, int progId)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO [dbo].[EquipmentProgram]([Program],[Equipment]) VALUES({progId},'{equip}')";
            command.ExecuteNonQuery();
        }
        public void UpdateEquipment(Guid id, int Auditory, int Status, int ResponsibleUserId, int TempResponsibleUserId, string Comment, int Count)
        {
            var eq = GetFullEquipment(id).Result;

            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"Update Equipment set AuditoryId={Auditory}, ResponsibleUserId = {ResponsibleUserId}, TempResponsibleUserId = {(TempResponsibleUserId==0?"null":TempResponsibleUserId)}" +
                $", StatusId = {Status}, Count = {Count} where InventNumber = '{id}'";
            command.ExecuteNonQuery();

            string commandText = "";

            if (eq.ResponsibleUserId!=ResponsibleUserId)
            {
                commandText = $"Insert into EquipmentUserStory(Equipment,[User],[Date],Comment) Values('{id}',{ResponsibleUserId},'{DateTime.Now}','{Comment}');";
            }
            if (eq.AuditoryId!=Auditory)
            {
                commandText+= $"Insert into [EquipmentAuditoryStory](Equipment,[AuditoryId],[Datetime],Comment) Values('{id}',{ResponsibleUserId},'{DateTime.Now}','{Comment}');";
            }
            if (commandText!="")
            {
                command.CommandText = commandText;
                command.ExecuteNonQuery();
            }

        }
        public void AddEquipmentSettings(Guid id, EquipmentSettings settings)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"Insert into EquipmentSettings(IPAddr,[Mask],[Getaway],DNS,EquipmentId) Values('{settings.IPAddr}','{settings.Mask}','{settings.Getaway}','{settings.DNS}','{id}');";
            command.ExecuteNonQuery();
        }
        public async Task<Equipment> GetEquipmentSettings(string ip)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select EquipmentId from [EquipmentSettings] where IPAddr = '{ip}'";
            var reader = await command.ExecuteReaderAsync();
            reader.Read();

            if (reader.HasRows)
            {
                return await GetShortEquipment(reader.GetGuid(0));
            }
            else
            {
                return null;
            }
        }
        public void AddAuditory(string Name, string ShortName, int ResponsibleUserId, int TempResponsibleUserId)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"Insert into Auditory([Name],[ShortName],[ResponsibleUserId],TempResponsibleUserId) Values('{Name}','{ShortName}',{ResponsibleUserId},{(TempResponsibleUserId==0?"null": TempResponsibleUserId)});";
            command.ExecuteNonQuery();
        }
        public void AddProgram(string Name, string Version, string Creator)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select Id from ProgramCreators where [Name] = '{Creator}'";
            var reader = command.ExecuteReader();
            reader.Read();

            int? creatorId = null;
            if (reader.HasRows)
            {
                creatorId = reader.GetInt32(0);
                reader.Close();
            } else
            {
                reader.Close();

                command.CommandText = $"Insert into ProgramCreators([Name]) values('{Creator}');";
                command.ExecuteNonQuery();

                command.CommandText = $"select Id from ProgramCreators where [Name] = '{Creator}'";
                reader = command.ExecuteReader();
                reader.Read();
                creatorId= reader.GetInt32(0);
                reader.Close();
            }
            

            command.CommandText = $"Insert into Programms([Name],[Version],Creators) values('{Name}','{Version}',{creatorId});";
            command.ExecuteNonQuery();
        }
        public void AddUser(User user)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO [dbo].[User]([Password],[Login],[UserRoleId],[Email],[SecondName],[Name],[LastName],[Phone],[Adres])VALUES('{user.Password}','{user.Login}','{user.UserRoleId}','{user.Email}','{user.SecondName}','{user.Name}','{user.LastName}','{user.Phone}','{user.Adres}')";
            command.ExecuteNonQuery();
        }
        public void UpdateAuditory(Auditory aud)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"update [dbo].[Auditory] set [Name] = '{aud.Name}',[ShortName] = '{aud.ShortName}',[ResponsibleUserId]={aud.ResponsibleUserId},[TempResponsibleUserId] = {(aud.TempResponsibleUserId!=0?aud.TempResponsibleUserId:"null")} where Id = {aud.Id}";
            command.ExecuteNonQuery();
        }
        public async Task<ProgramClass> GetProgram(int id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select p.Name,p.Version,pc.Name from [Programms] as p, ProgramCreators as pc where pc.Id = p.Creators and p.Id = {id}";
            var reader = await command.ExecuteReaderAsync();
            reader.Read();

            var prog = new ProgramClass()
            {
                Id = id,
                Name = reader.GetString(0),
                Version = reader.GetString(1),
            };
            prog.ProgramCreator.Name = reader.GetString(2);

            return prog;
        }
        public void UpdateProgram(int id,string Name, string Version, string Creator)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select Id from ProgramCreators where [Name] = '{Creator}'";
            var reader = command.ExecuteReader();
            reader.Read();

            int? creatorId = null;
            if (reader.HasRows)
            {
                creatorId = reader.GetInt32(0);
                reader.Close();
            }
            else
            {


                command.CommandText = $"Insert into ProgramCreators([Name]) values('{Creator}');";
                command.ExecuteNonQuery();

                command.CommandText = $"select Id from ProgramCreators where [Name] = '{Creator}'";
                reader = command.ExecuteReader();
                reader.Read();
                creatorId = reader.GetInt32(0);
                reader.Close();
            }


            command.CommandText = $"Update Programms set [Name]='{Name}',[Version]='{Version}',Creators = {creatorId} where Id = {id} ;";
            command.ExecuteNonQuery();
        }
        public void UpdateUser(int id, User user)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"Update [dbo].[User] set [Password]='{user.Password}',[Login]='{user.Login}',[UserRoleId]={user.UserRoleId},[Email]='{user.Email}',[SecondName]='{user.SecondName}',[Name]='{user.Name}',[LastName]='{user.LastName}',[Phone]='{user.Phone}',[Adres]='{user.Adres}' where Id = {id}";
            command.ExecuteNonQuery();
        }
        public async Task<User> GetFullUser(int id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT [Id],[UserRoleId],[Email],[SecondName],[Name],[LastName],[Phone],[Adres],[Password],[Login] FROM [InventarisationDB].[dbo].[User] where Id = {id}";

            var reader = await command.ExecuteReaderAsync();
            reader.Read();
            if (reader.HasRows)
            {
                User user = new User()
                {
                    Id = reader.GetInt32(0),
                    UserRoleId = reader.GetInt32(1),
                    Email = reader.GetString(2),
                    SecondName = reader.GetString(3),
                    Name = reader.GetString(4),
                    LastName = reader.GetString(5),
                    Phone = reader.GetString(6),
                    Adres = reader.GetString(7),
                    Password = reader.GetString(8),
                    Login = reader.GetString(9),

                };
                return user;
            }
            return null;
        }
        public int? AddEquipmentModel(string Name, string Type)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select Id from EquipmentType where [Name] = '{Type}'";
            var reader = command.ExecuteReader();
            reader.Read();

            int? creatorId = null;
            int? returnValue = null;
            if (reader.HasRows)
            {
                creatorId = reader.GetInt32(0);
                reader.Close();
            }
            else
            {
                reader.Close();

                command.CommandText = $"Insert into EquipmentType([Name]) values('{Type}');";
                command.ExecuteNonQuery();

                command.CommandText = $"select Id from EquipmentType where [Name] = '{Type}'";
                reader = command.ExecuteReader();
                reader.Read();
                creatorId = reader.GetInt32(0);
                reader.Close();

                returnValue = creatorId;
            }


            command.CommandText = $"Insert into EquipmentModel([Name],[EquipmentTypeId]) values('{Name}',{creatorId});";
            command.ExecuteNonQuery();

            return returnValue;
        }

        public void AddDirection(string Name)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select Id from Direction where [Name] = '{Name}'";
            var reader = command.ExecuteReader();
            reader.Read();

            int? creatorId = null;
            if (reader.HasRows)
            {
                creatorId = reader.GetInt32(0);
                reader.Close();
            }
            else
            {
                reader.Close();

                command.CommandText = $"Insert into Direction([Name]) values('{Name}');";
                command.ExecuteNonQuery();
            }
        }

        public async Task<List<Inventariation>> GetInventariations()
        {
            List <Inventariation> inventariations = new List <Inventariation>();

            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT * from Inventarisation";

            var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                Inventariation inventariation = new()
                {
                    Id = reader.GetInt32(0),
                    Start = reader.GetDateTime(1),
                    End= reader.GetDateTime(2),
                    Name = reader.GetString(3)
                };

                inventariations.Add(inventariation);
            }
            return inventariations;
        }

        public async Task<Inventariation> GetInventariation(int id, int? userId =null)
        {
            Inventariation inventariation = new Inventariation();
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT * from Inventarisation where Id = {id}";
            var reader = await command.ExecuteReaderAsync();

            reader.Read();
            inventariation.Id = id;
            inventariation.Start = reader.GetDateTime(1);
            inventariation.End = reader.GetDateTime(2);
            inventariation.Name = reader.GetString(3);

            reader.Close();

            command.CommandText = $"select * from InventarisationEquipment where InventarisationId = {id}";
            reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var equipment = await GetFullEquipment(reader.GetGuid(2));

                InventarisationEquipment inventarisationEquipment = new(equipment);
                inventarisationEquipment.InventEquipId = reader.GetInt32(0); 
                inventarisationEquipment.Count = reader.GetInt32(4);
                inventarisationEquipment.Checked = Convert.ToBoolean(reader.GetByte(3));

                if (userId == null)
                {
                    inventariation.Equipment.Add(inventarisationEquipment);
                } else if (userId == equipment.ResponsibleUserId || userId == equipment.TempResponsibleUserId)
                {
                    inventariation.Equipment.Add(inventarisationEquipment);
                }
            }
            reader.Close();

            if (userId == null)
            {
                command.CommandText = $"select * from InventarisationUser where InventarisationId = {id}";
                reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    var user = await GetUser(reader.GetInt32(2));
                    inventariation.Users.Add(user);
                }
                reader.Close();
            }

            return inventariation;
        }

        public async Task<bool> GetInventariationContain(int inventId, int userId)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT * from InventarisationUser where InventarisationId ={inventId} and UserId = {userId}";
            var reader = await command.ExecuteReaderAsync();
            return reader.HasRows;
        }

        public void AddUserToInventarisation(int inventId, int userId)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"insert Into InventarisationUser([InventarisationId],[UserId]) values({inventId},{userId})";
            command.ExecuteNonQuery();
        }
        public void UpdateInventarisationEquipment(int id,int count)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"Update InventarisationEquipment set [Count] = {count}, Checked = 1 where Id = {id}";
            command.ExecuteNonQuery();
        }

        public async void AddInventarisation(AddInventarisationViewModel viewModel)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"insert into [dbo].[Inventarisation]([Start],[End],[Name]) values('{viewModel.Start}','{viewModel.End}','{viewModel.Name}'); SELECT SCOPE_IDENTITY();";
            int insertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

            string commandText = "";
            foreach (var item in viewModel.EquipmentNumbers)
            {
                commandText += $"INSERT INTO [dbo].[InventarisationEquipment]([InventarisationId],[EquipmentNumber],[Checked],[Count]) values({insertedId},'{item}',0,0);";
            }
            if (commandText!="")
            {
                command.CommandText = commandText;
                command.ExecuteNonQuery();
            }
        }

        public async Task<Consumables> GetConsumable(int id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select c.[Id],c.[Name],c.[Description],c.[DateOfCame],c.[Image],c.[Count],c.[ResponsibleUserId],c.[TempResponsibleUserId],c.[ConsumablesTypeId], ct.[name] " +
                $"from [Consumables] as c, [ConsumablesType] as ct where c.[ConsumablesTypeId] = ct.Id and c.Id = {id}";
            var reader = await command.ExecuteReaderAsync();
            reader.Read();
            Consumables consumable = new()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                DateOfCame = reader.GetDateTime(3),
                Image = reader.IsDBNull(4) ? null : (byte[])reader.GetValue(4),
                Count = reader.GetInt32(5),
                ResponsibleUserId = reader.GetInt32(6),
                TempResponsibleUserId = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
            };
            consumable.ConsumablesType.Id = reader.GetInt32(8);
            consumable.ConsumablesType.Name = reader.GetString(9);

            var secondCommand = connection.CreateCommand();
            secondCommand.CommandText = $"select s.Id,s.[Name], csv.[Value] " +
                $"from [ConsumablesTypeSpecifications] as cts, Specifications as s,ConsumablesSpecificationsValues as csv " +
                $"where cts.ConsumablesTypeId = {consumable.ConsumablesType.Id} and s.Id = cts.SpecificationsId and csv.SpecificationId = s.Id and csv.ConsumableId = {consumable.Id}";
            var secondReader = await secondCommand.ExecuteReaderAsync();

            while (secondReader.Read())
            {
                Specifications specifications = new()
                {
                    Id = secondReader.GetInt32(0),
                    Name = secondReader.GetString(1),
                    Value = secondReader.GetString(2),
                };

                consumable.Specifications.Add(specifications);
            }

            secondReader.Close();

            consumable.ResponsibleUser = await GetUser(consumable.ResponsibleUserId);

            if (consumable.TempResponsibleUserId != 0)
            {
                consumable.TempResponsibleUser = await GetUser(consumable.TempResponsibleUserId);
            }

            return consumable;
        }

        public async Task<List<ConsumablesType>> GetConsumableTypes()
        {
            List<ConsumablesType> consumablesTypes= new List<ConsumablesType>();
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT * from ConsumablesType";
            var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                ConsumablesType consumablesType = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                };
                consumablesTypes.Add(consumablesType);
            }

            return consumablesTypes;
        }

        public async Task<List<Specifications>> GetConsumableSpecifications(int id)
        {
            List<Specifications> consumablesSpecifications = new List<Specifications>();
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT s.Id, s.[Name] from ConsumablesTypeSpecifications as cts, Specifications as s where s.Id = cts.SpecificationsId and cts.ConsumablesTypeId = {id}";
            var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                Specifications consumablesType = new()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                };
                consumablesSpecifications.Add(consumablesType);
            }

            return consumablesSpecifications;
        }

        public async Task<int> AddConsumableTypeAsync(string Name)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"insert into [dbo].[ConsumablesType]([Name]) values('{Name}'); SELECT SCOPE_IDENTITY();";
            int insertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

            return insertedId;
        }

        public async Task<int> AddConsumableSpecifications(string Name, int ConsumableId)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();

            command.CommandText = $"select Id from Specifications where [Name]='{Name}'";
            var reader = await command.ExecuteReaderAsync();

            int specId = 0;
            if (reader.HasRows)
            {
                reader.Read();
                specId = reader.GetInt32(0);
                reader.Close();
            }
            else
            {
                reader.Close();
                command.CommandText = $"insert into [dbo].[Specifications]([Name]) values('{Name}'); SELECT SCOPE_IDENTITY();";
                specId = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            command.CommandText = $"insert into [dbo].[ConsumablesTypeSpecifications]([ConsumablesTypeId],[SpecificationsId]) values({ConsumableId},{specId}); SELECT SCOPE_IDENTITY();";
            command.ExecuteNonQuery();

            return specId;
        }

        public void AddConsumable(Consumables Consumable, List<ConsumablesSpecificationsValues> values)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();

            command.CommandText = $"insert into Consumables([Name],[Description],[DateOfCame],[Image],[Count],[ResponsibleUserId],[TempResponsibleUserId],[ConsumablesTypeId]) " +
                $"values ('{Consumable.Name}','{Consumable.Description}','{Consumable.DateOfCame}',{(Consumable.Image != null ? "@Image" : "null")},{Consumable.Count},{Consumable.ResponsibleUserId}," +
                $"{(Consumable.TempResponsibleUserId != 0 ? Consumable.TempResponsibleUserId : "null")},{Consumable.ConsumableTypeId}); SELECT SCOPE_IDENTITY();";

            if (Consumable.Image!=null)
            {
                command.Parameters.Add("@Image",SqlDbType.Image);
                command.Parameters["@Image"].Value = Consumable.Image;
            }

            int specId = Convert.ToInt32(command.ExecuteScalar());

            string valuesCommand = "";

            foreach (var item in values)
            {
                valuesCommand += $"insert into ConsumablesSpecificationsValues(SpecificationId,ConsumableId,[Value]) values({item.SpecificationId},{specId},'{item.Value}')";
            }

            if (valuesCommand!="")
            {
                command.CommandText = valuesCommand;
                command.ExecuteNonQuery();
            }
        }

        public void UpdateConsumable(int Id, int Count, int ResponsibleUserId, int TempResponsibleUserId)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"Update [dbo].[Consumables] set [Count]={Count}, ResponsibleUserId= {ResponsibleUserId}, TempResponsibleUserId = {(TempResponsibleUserId!=0?TempResponsibleUserId:"null")} where Id = {Id}";
            command.ExecuteNonQuery();
        }

        public async Task<List<StoryItem>> StoryOfMove(Guid Id)
        {
            List<StoryItem> StoryItems = new List<StoryItem>();
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT s.[Datetime],s.[Comment],e.[Name],a.[Name] from EquipmentAuditoryStory as s, Equipment as e, Auditory as a " +
                $"where s.Equipment = '{Id}' and e.InventNumber = '{Id}' and a.Id = s.AuditoryId order by s.[Datetime]";
            var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                StoryItem consumablesType = new()
                {
                    DateOfMove = reader.GetDateTime(0),
                    Comment = reader.GetString(1),
                    EquipName= reader.GetString(2),
                    Delegate = reader.GetString(3),
                };
                StoryItems.Add(consumablesType);
            }

            return StoryItems;
        }

        public async Task<List<StoryItem>> StoryOfResponce(Guid Id)
        {
            List<StoryItem> StoryItems = new List<StoryItem>();
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT s.[Date],s.[Comment],e.[Name],u.[Name] from EquipmentUserStory as s, Equipment as e, [User] as u " +
                $"where s.Equipment = '{Id}' and u.Id = s.[User] and e.InventNumber = '{Id}'  order by s.[Date]";
            var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                StoryItem consumablesType = new()
                {
                    DateOfMove = reader.GetDateTime(0),
                    Comment = reader.GetString(1),
                    EquipName = reader.GetString(2),
                    Delegate = reader.GetString(3),
                };
                StoryItems.Add(consumablesType);
            }

            return StoryItems;
        }

        public void DeleteAuditory(int Id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"delete from Auditory where Id = {Id}";
            command.ExecuteNonQuery();
        }

        public void DeleteConsumable(int Id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"delete from Consumables where Id = {Id}";
            command.ExecuteNonQuery();
        }

        public void DeleteEquipment(Guid Id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"update Equipment set PseudoDelete = 1 where InventNumber = '{Id}'";
            command.ExecuteNonQuery();
        }

        public void DeleteInventarisation(int Id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"delete from Inventarisation where Id = {Id}";
            command.ExecuteNonQuery();
        }

        public void DeleteProgram(int Id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"delete from Programms where Id = {Id}";
            command.ExecuteNonQuery();
        }

        public void DeleteUser(int Id)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"Update [User] set PseudoDelited = 1 where Id = {Id}";
            command.ExecuteNonQuery();
        }

        public void AddEquipTypeConsumables(int Type, List<int> cons)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            string valuesCommand = "";

            foreach (var item in cons)
            {
                valuesCommand += $"insert into EquipmentConsumables(EquipmentType,Consumable) values({Type},{item})";
            }

            if (valuesCommand != "")
            {
                command.CommandText = valuesCommand;
                command.ExecuteNonQuery();
            }
        }

        public async Task<int> GetUserByFullName(string Name, string SecondName, string LastName)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select Id from [User] where [Name] = '{Name}' and SecondName='{SecondName}' and LastName = '{LastName}'";
            var reader = command.ExecuteReader();
            reader.Read();

            int userId = 0;
            if (reader.HasRows)
            {
                userId = reader.GetInt32(0);
                reader.Close();
            }
            else
            {
                reader.Close();

                command.CommandText = $"Insert into [User]([Password],[Login],[UserRoleId],[SecondName],[Name],[LastName]) " +
                    $"values('user','user',3,'{SecondName}','{Name}','{LastName}'); SELECT SCOPE_IDENTITY();";
                userId = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            return userId;
        }

        public async Task<int> GetEquipmentTypeByNameAsync(string Name)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select Id from EquipmentType where [Name] = '{Name}'";
            var reader = command.ExecuteReader();
            reader.Read();

            int creatorId = 0;
            if (reader.HasRows)
            {
                creatorId = reader.GetInt32(0);
                reader.Close();
            }
            else
            {
                reader.Close();

                command.CommandText = $"Insert into EquipmentType([Name]) values('{Name}'); SELECT SCOPE_IDENTITY();";
                creatorId = Convert.ToInt32(await command.ExecuteScalarAsync());
            }


            return creatorId;
        }

        public async Task<int> GetEquipmentModel(string Name, int Type)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select [Id] from EquipmentModel where [Name] = '{Name}'";
            var reader = command.ExecuteReader();
            reader.Read();

            int creatorId = 0;
            if (reader.HasRows)
            {
                creatorId = reader.GetInt32(0);
                reader.Close();
            }
            else
            {
                reader.Close();

                command.CommandText = $"Insert into EquipmentModel([Name],[EquipmentTypeId]) values('{Name}',{Type}); SELECT SCOPE_IDENTITY();";
                creatorId = Convert.ToInt32(await command.ExecuteScalarAsync());

            }

            return creatorId;
        }

        public async Task<int> GetDirectionId(string Name)
        {
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"select Id from Direction where [Name] = '{Name}'";
            var reader = command.ExecuteReader();
            reader.Read();

            int creatorId = 0;
            if (reader.HasRows)
            {
                creatorId = reader.GetInt32(0);
                reader.Close();
            }
            else
            {
                reader.Close();

                command.CommandText = $"Insert into Direction([Name]) values('{Name}');SELECT SCOPE_IDENTITY();";
                creatorId = Convert.ToInt32(await command.ExecuteScalarAsync());
            }
            return creatorId;
        }

        public async void AddImportedEquipmentAsync(string Name, int ResponsibleUserId, int EquipmentModelId, int DirectionId, int Count)
        {
            Guid guid = Guid.NewGuid();
            using var connection = GetConnection();
             
            var command = connection.CreateCommand();
            command.CommandText = $"insert into [dbo].[Equipment]([InventNumber],[Name],[ResponsibleUserId]" +
                $",[TempResponsibleUserId],[AuditoryId],[Price]" +
                $",[StatusId],[EquipmentModelId],[DirectionId],[Count])" +
                $" values('{guid}','{Name}',{ResponsibleUserId}, null" +
                $",null,0,4,{EquipmentModelId},{DirectionId},{Count})";
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Equipment>> GetDirectionEquipment(int DirId)
        {
            using var connection = GetConnection();
             
            List<Equipment> equipments = new List<Equipment>();
            var command = connection.CreateCommand();
            command.CommandText = $"select [Name],[Price],[Count] from Equipment where [DirectionId] = '{DirId}'";
            var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                Equipment equipment = new()
                {
                    Name=reader.GetString(0),
                    Price  = reader.GetDouble(1),
                    Count= reader.GetInt32(2)
                };
                equipments.Add(equipment);
            }
            return equipments;
        }
    }
}
