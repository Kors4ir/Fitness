using System;
using System.Collections.Generic;
using System.Linq;

namespace FitnessCenter
{
    public class FitnessManager
    {
        private List<Client> clients = new List<Client>();
        private List<Membership> memberships = new List<Membership>();
        private List<TrainingRoom> trainingRooms = new List<TrainingRoom>();
        private List<Instructor> instructors = new List<Instructor>();
        
        private int nextClientId = 1000;
        private int nextMembershipId = 1;
        private decimal dailyRevenue = 0;
        private int dailyVisits = 0;
        private DateTime currentDay = DateTime.Today;
        
        // TODO 1: Добавить клиента
        public void AddClient(Client client)
        {
            clients.Add(client);
            Console.WriteLine($"Клиент {client.FullName} добавлен в систему.");
        }
        
        // TODO 1: Добавить абонемент
        public void AddMembership(Membership membership)
        {
            memberships.Add(membership);
        }
        
        // TODO 1: Добавить зал
        public void AddTrainingRoom(TrainingRoom room)
        {
            trainingRooms.Add(room);
        }
        
        // TODO 1: Добавить тренера
        public void AddInstructor(Instructor instructor)
        {
            instructors.Add(instructor);
        }
        
        // TODO 2: Найти клиента по телефону
        public Client FindClientByPhone(string phone)
        {
            return clients.FirstOrDefault(c => c.Phone == phone);
        }
        
        // TODO 2: Продать абонемент клиенту
        public bool SellMembershipToClient(Client client, string membershipType, int durationDays)
        {
            if (client == null) return false;
            
            // Проверяем, нет ли уже действующего абонемента
            // Здесь мы просто создаем новый
            
            decimal price = CalculateMembershipPrice(membershipType, durationDays);
            
            // Определяем количество посещений
            int visitsAllowed = 0; // 0 = безлимит
            if (membershipType.ToLower() == "разовый")
                visitsAllowed = 1;
                
            // Определяем включенные услуги
            string services = "зал";
            if (membershipType.ToLower() == "месячный" || membershipType.ToLower() == "годовой")
                services = "зал,бассейн,групповые";
            else if (membershipType.ToLower() == "студенческий")
                services = "зал,групповые";
            
            Membership membership = new Membership(
                GetNextMembershipId(),
                membershipType,
                price,
                DateTime.Now,
                visitsAllowed,
                services
            );
            
            if (client.AddMembership(membership))
            {
                AddMembership(membership);
                dailyRevenue += price;
                Console.WriteLine($"Абонемент продан за {price} руб. Выручка за день: {dailyRevenue} руб.");
                return true;
            }
            
            return false;
        }
        
        // TODO 2: Зарегистрировать посещение
        public bool RegisterClientVisit(Client client, string service, int roomId)
        {
            if (client == null) return false;
            
            // Проверяем новый день
            if (currentDay != DateTime.Today)
            {
                currentDay = DateTime.Today;
                dailyRevenue = 0;
                dailyVisits = 0;
            }
            
            // Находим зал
            var room = trainingRooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null)
            {
                Console.WriteLine("Зал не найден.");
                return false;
            }
            
            // Регистрируем посещение
            if (client.RegisterVisit(service, 60)) // По умолчанию 60 минут
            {
                if (room.RegisterClientEntry())
                {
                    dailyVisits++;
                    Console.WriteLine($"Посещение зарегистрировано. Загрузка зала: {room.GetCurrentLoad()}/{room.Capacity}");
                    return true;
                }
                else
            {
                    Console.WriteLine("Зал переполнен.");
                    return false;
                }
            }
            
            return false;
        }
        
        // TODO 3: Найти доступного тренера
        public Instructor FindAvailableInstructor(DateTime desiredTime, string specialization = "")
        {
            var available = instructors.Where(i => 
                i.IsAvailable(desiredTime, 60) && 
                (string.IsNullOrEmpty(specialization) || i.Specialization.Contains(specialization))
            ).ToList();
            
            return available.FirstOrDefault();
        }
        
        // TODO 3: Получить финансовую статистику
        public (decimal dailyRevenue, decimal monthlyRevenue, int activeMemberships, double avgDailyVisits) GetFinancialStats()
        {
            // Выручка за день
            decimal dailyRev = dailyRevenue;
            
            // Оценочная выручка за месяц
            decimal monthlyRev = dailyRev * 30;
            
            // Активные абонементы
            int active = memberships.Count(m => m.IsValid());
            
            // Среднее количество посещений в день (за последние 7 дней)
            double avgVisits = dailyVisits; // В реальном проекте нужно считать историю
            
            return (dailyRev, monthlyRev, active, avgVisits);
        }
        
        // TODO 3: Получить залы с высокой загрузкой
        public List<TrainingRoom> GetBusyRooms(int thresholdPercent = 80)
        {
            return trainingRooms
                .Where(r => r.GetLoadPercentage() >= thresholdPercent)
                .ToList();
        }
        
        // Готовые методы:
        public List<Client> GetAllClients()
        {
            return clients;
        }
        
        public List<TrainingRoom> GetAllRooms()
        {
            return trainingRooms;
        }
        
        public List<Instructor> GetAllInstructors()
        {
            return instructors;
        }
        
        public int GetNextClientId()
        {
            return nextClientId++;
        }
        
        public int GetNextMembershipId()
        {
            return nextMembershipId++;
        }
        
        public decimal CalculateMembershipPrice(string type, int durationDays)
        {
            switch (type.ToLower())
            {
                case "разовый": return 500;
                case "месячный": return 3000;
                case "годовой": return 25000;
                case "студенческий": return 2000;
                default: return 2000;
            }
        }
    }
}