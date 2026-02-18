using System;
using System.Linq;

namespace FitnessCenter
{
    public class FitnessMenu
    {
        private FitnessManager manager;
        
        public FitnessMenu()
        {
            manager = new FitnessManager();
            InitializeData();
        }
        
        private void InitializeData()
        {
            // Инициализация залов
            manager.AddTrainingRoom(new TrainingRoom(1, "Силовой зал", 30, "06:00-23:00", "гантели, штанги, тренажеры"));
            manager.AddTrainingRoom(new TrainingRoom(2, "Кардио-зал", 20, "06:00-23:00", "беговые дорожки, велотренажеры, эллипсы"));
            manager.AddTrainingRoom(new TrainingRoom(3, "Групповой зал", 25, "08:00-22:00", "коврики, фитболы, бодибары"));
            manager.AddTrainingRoom(new TrainingRoom(4, "Бассейн", 15, "07:00-21:00", "дорожки, доски для плавания"));
            
            // Инициализация тренеров
            manager.AddInstructor(new Instructor(1, "Иванов Алексей", "силовые тренировки", 
                                               "+79001112233", "персональный тренер, диетолог", "10:00-20:00"));
            manager.AddInstructor(new Instructor(2, "Петрова Мария", "йога и стретчинг", 
                                               "+79002223344", "инструктор групповых программ, йога", "08:00-18:00"));
            manager.AddInstructor(new Instructor(3, "Сидоров Дмитрий", "кардио и функциональный тренинг", 
                                               "+79003334455", "мастер спорта, тренер по бегу", "12:00-22:00"));
            
            // Инициализация клиента с абонементом
            Client client = new Client(manager.GetNextClientId(), "Кузнецова Ольга", 
                                      "+79005556677", new DateTime(1990, 5, 15), 168, 65, "похудение");
            manager.AddClient(client);
            
            Membership membership = new Membership(manager.GetNextMembershipId(), "месячный", 
                                                  3000, DateTime.Now, 0, "зал,групповые");
            client.AddMembership(membership);
            manager.AddMembership(membership);
            
            // Добавляем тестовые посещения
            client.RegisterVisit("зал", 75);
            client.RegisterVisit("групповые", 60);
            client.AddTrainingRecord("Жим лежа", 40, 10, 3);
            client.AddTrainingRecord("Приседания", 50, 12, 3);
        }
        
        // TODO 1: Зарегистрировать нового клиента
        public void RegisterNewClient()
        {
            Console.WriteLine("=== РЕГИСТРАЦИЯ НОВОГО КЛИЕНТА ===");
            
            Console.Write("ФИО: ");
            string name = Console.ReadLine();
            
            Console.Write("Телефон: ");
            string phone = Console.ReadLine();
            
            Console.Write("Дата рождения (дд.мм.гггг): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime birthDate))
            {
                Console.WriteLine("Неверный формат даты");
                return;
            }
            
            Console.Write("Рост (см): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal height))
            {
                Console.WriteLine("Неверный формат роста");
                return;
            }
            
            Console.Write("Вес (кг): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal weight))
            {
                Console.WriteLine("Неверный формат веса");
                return;
            }
            
            Console.WriteLine("Цель тренировок:");
            Console.WriteLine("1. Похудение");
            Console.WriteLine("2. Набор мышечной массы");
            Console.WriteLine("3. Поддержание формы");
            Console.WriteLine("4. Реабилитация");
            Console.Write("Выберите цель: ");
            string goal = Console.ReadLine() switch
            {
                "1" => "похудение",
                "2" => "набор массы",
                "3" => "поддержание",
                "4" => "реабилитация",
                _ => "поддержание"
            };
            
            Client client = new Client(manager.GetNextClientId(), name, phone, 
                                      birthDate, height, weight, goal);
            manager.AddClient(client);
            
            Console.WriteLine($"\nКлиент зарегистрирован. ID: {client.Id}");
            client.ShowClientInfo();
            
            // Предложить приобрести абонемент
            Console.Write("\nХотите приобрести абонемент? (да/нет): ");
            if (Console.ReadLine().ToLower() == "да")
            {
                SellMembershipToClient(client);
            }
        }
        
        // TODO 1: Продать абонемент
        public void SellMembershipToClient(Client client = null)
        {
            Console.WriteLine("=== ПРОДАЖА АБОНЕМЕНТА ===");
            
            if (client == null)
            {
                Console.Write("Введите телефон клиента: ");
                string phone = Console.ReadLine();
                client = manager.FindClientByPhone(phone);
                
                if (client == null)
                {
                    Console.WriteLine("Клиент не найден");
                    return;
                }
            }
            
            Console.WriteLine($"Клиент: {client.FullName}");
            client.ShowClientInfo();
            
            Console.WriteLine("\nДоступные типы абонементов:");
            Console.WriteLine("1. Разовый (500 руб.) - 1 посещение");
            Console.WriteLine("2. Месячный (3000 руб.) - безлимит, зал+бассейн+групповые");
            Console.WriteLine("3. Годовой (25000 руб.) - безлимит, все услуги");
            Console.WriteLine("4. Студенческий (2000 руб./мес) - безлимит, зал+групповые");
            Console.Write("Выберите тип: ");
            
            string choice = Console.ReadLine();
            string membershipType = choice switch
            {
                "1" => "разовый",
                "2" => "месячный",
                "3" => "годовой",
                "4" => "студенческий",
                _ => "месячный"
            };
            
            if (manager.SellMembershipToClient(client, membershipType, 30))
            {
                Console.WriteLine($"✅ Абонемент '{membershipType}' оформлен для {client.FullName}");
            }
            else
            {
                Console.WriteLine("❌ Не удалось оформить абонемент");
            }
        }
        
        // TODO 2: Зарегистрировать посещение
        public void RegisterVisit()
        {
            Console.WriteLine("=== РЕГИСТРАЦИЯ ПОСЕЩЕНИЯ ===");
            
            Console.Write("Введите телефон клиента: ");
            string phone = Console.ReadLine();
            
            Client client = manager.FindClientByPhone(phone);
            if (client == null)
            {
                Console.WriteLine("❌ Клиент не найден");
                return;
            }
            
            Console.WriteLine("\nДоступные услуги:");
            Console.WriteLine("1. Тренажерный зал");
            Console.WriteLine("2. Кардио-зал");
            Console.WriteLine("3. Групповые занятия");
            Console.WriteLine("4. Бассейн");
            Console.Write("Выберите услугу: ");
            
            string serviceChoice = Console.ReadLine();
            string service = serviceChoice switch
            {
                "1" => "зал",
                "2" => "зал", // Кардио-зал тоже считается "зал"
                "3" => "групповые",
                "4" => "бассейн",
                _ => "зал"
            };
            
            Console.Write("Продолжительность посещения (минут): ");
            if (!int.TryParse(Console.ReadLine(), out int duration))
            {
                duration = 60;
            }
            
            Console.WriteLine("Доступные залы:");
            foreach (var room in manager.GetAllRooms())
            {
                Console.WriteLine($"  {room.Id}. {room.Name} - {room.GetCurrentLoad()}/{room.Capacity}");
            }
            Console.Write("Выберите ID зала: ");
            if (!int.TryParse(Console.ReadLine(), out int roomId))
            {
                Console.WriteLine("Неверный ID зала");
                return;
            }
            
            if (manager.RegisterClientVisit(client, service, roomId))
            {
                Console.WriteLine($"✅ Посещение зарегистрировано для {client.FullName}");
            }
            else
            {
                Console.WriteLine("❌ Не удалось зарегистрировать посещение");
            }
        }
        
        // TODO 2: Показать информацию о залах
        public void ShowRoomsInfo()
        {
            Console.WriteLine("=== ЗАЛЫ ФИТНЕС-ЦЕНТРА ===");
            
            var rooms = manager.GetAllRooms();
            foreach (var room in rooms)
            {
                room.ShowRoomInfo();
                Console.WriteLine();
            }
            
            var busyRooms = manager.GetBusyRooms(50);
            if (busyRooms.Count > 0)
            {
                Console.WriteLine("⚠️ Залы с высокой загрузкой:");
                foreach (var room in busyRooms)
                {
                    Console.WriteLine($"  - {room.Name}: {room.GetLoadPercentage()}%");
                }
            }
        }
        
        // TODO 2: Показать информацию о тренерах
        public void ShowInstructorsInfo()
        {
            Console.WriteLine("=== ТРЕНЕРЫ ФИТНЕС-ЦЕНТРА ===");
            
            var instructors = manager.GetAllInstructors();
            foreach (var instructor in instructors)
            {
                instructor.ShowInstructorInfo();
                Console.WriteLine();
            }
        }
        
        // TODO 3: Записаться на персональную тренировку
        public void BookPersonalTraining()
        {
            Console.WriteLine("=== ЗАПИСЬ НА ПЕРСОНАЛЬНУЮ ТРЕНИРОВКУ ===");
            
            Console.Write("Введите телефон клиента: ");
            string phone = Console.ReadLine();
            
            Client client = manager.FindClientByPhone(phone);
            if (client == null)
            {
                Console.WriteLine("❌ Клиент не найден");
                return;
            }
            
            Console.Write("Желаемая дата и время (дд.мм.гггг чч:мм): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime desiredTime))
            {
                Console.WriteLine("Неверный формат даты");
                return;
            }
            
            Console.Write("Специализация тренера (оставьте пустым для любого): ");
            string specialization = Console.ReadLine();
            
            var instructor = manager.FindAvailableInstructor(desiredTime, specialization);
            if (instructor == null)
            {
                Console.WriteLine("❌ Нет доступных тренеров на указанное время");
                return;
            }
            
            Console.WriteLine($"\nНайден тренер: {instructor.FullName}");
            Console.WriteLine($"Специализация: {instructor.Specialization}");
            Console.WriteLine($"Квалификации: {instructor.Qualifications}");
            Console.Write("\nПодтвердить запись? (да/нет): ");
            
            if (Console.ReadLine().ToLower() == "да")
            {
                Console.Write("Фокус тренировки (силовая, кардио, растяжка): ");
                string focus = Console.ReadLine();
                
                if (instructor.AddPersonalSession(client, desiredTime, 60, focus))
                {
                    Console.WriteLine("✅ Тренировка записана!");
                }
                else
                {
                    Console.WriteLine("❌ Не удалось записать тренировку");
                }
            }
        }
        
        // TODO 3: Показать отчет
        public void ShowFitnessReport()
        {
            Console.WriteLine("=== ОТЧЕТ ФИТНЕС-ЦЕНТРА ===");
            
            var stats = manager.GetFinancialStats();
            
            Console.WriteLine($"📊 ФИНАНСОВАЯ СТАТИСТИКА:");
            Console.WriteLine($"  Выручка за день: {stats.dailyRevenue:N0} руб.");
            Console.WriteLine($"  Прогноз на месяц: {stats.monthlyRevenue:N0} руб.");
            Console.WriteLine($"  Активных абонементов: {stats.activeMemberships}");
            Console.WriteLine($"  Среднее посещений в день: {stats.avgDailyVisits:F0}");
            
            Console.WriteLine($"\n🏋️ ЗАЛЫ С ВЫСОКОЙ ЗАГРУЗКОЙ (>80%):");
            var busyRooms = manager.GetBusyRooms(80);
            if (busyRooms.Count > 0)
            {
                foreach (var room in busyRooms)
                {
                    Console.WriteLine($"  {room.Name} - {room.GetLoadPercentage()}% ({room.GetCurrentLoad()}/{room.Capacity})");
                }
            }
            else
            {
                Console.WriteLine("  Нет залов с критической загрузкой");
            }
            
            Console.WriteLine($"\n👥 КЛИЕНТЫ:");
            var allClients = manager.GetAllClients();
            Console.WriteLine($"  Всего клиентов: {allClients.Count}");
            
            var withMembership = allClients.Count(c => {
                try { return c != null; } catch { return false; }
            });
            Console.WriteLine($"  С абонементом: примерно {withMembership}");
            
            var avgAge = allClients.Select(c => c.CalculateAge()).DefaultIfEmpty(0).Average();
            Console.WriteLine($"  Средний возраст: {avgAge:F0} лет");
        }
        
        // TODO 1: Найти клиента
        public void FindClient()
        {
            Console.Write("Введите телефон клиента: ");
            string phone = Console.ReadLine();
            
            Client client = manager.FindClientByPhone(phone);
            if (client != null)
            {
                client.ShowClientInfo();
            }
            else
            {
                Console.WriteLine("Клиент не найден");
            }
        }
        
        // Готовый метод - главное меню
        public void ShowMainMenu()
        {
            bool running = true;
            
            while (running)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║     ФИТНЕС-ЦЕНТР 'СИЛА И ГРАЦИЯ'      ║");
                Console.WriteLine("╠════════════════════════════════════════╣");
                Console.WriteLine("║  1. Регистрация нового клиента        ║");
                Console.WriteLine("║  2. Продажа абонемента                ║");
                Console.WriteLine("║  3. Регистрация посещения             ║");
                Console.WriteLine("║  4. Залы фитнес-центра                ║");
                Console.WriteLine("║  5. Тренеры                           ║");
                Console.WriteLine("║  6. Запись на персональную тренировку ║");
                Console.WriteLine("║  7. Отчет фитнес-центра               ║");
                Console.WriteLine("║  8. Найти клиента                     ║");
                Console.WriteLine("║  9. Добавить запись о тренировке      ║");
                Console.WriteLine("║  0. Выход                             ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.Write("Выберите: ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        RegisterNewClient();
                        break;
                    case "2":
                        SellMembershipToClient();
                        break;
                    case "3":
                        RegisterVisit();
                        break;
                    case "4":
                        ShowRoomsInfo();
                        break;
                    case "5":
                        ShowInstructorsInfo();
                        break;
                    case "6":
                        BookPersonalTraining();
                        break;
                    case "7":
                        ShowFitnessReport();
                        break;
                    case "8":
                        FindClient();
                        break;
                    case "9":
                        AddTrainingRecord();
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
                
                if (running && choice != "0")
                {
                    Console.WriteLine("\nНажмите Enter...");
                    Console.ReadLine();
                }
            }
        }
        
        // Дополнительный метод для добавления тренировки
        private void AddTrainingRecord()
        {
            Console.WriteLine("=== ДОБАВЛЕНИЕ ЗАПИСИ О ТРЕНИРОВКЕ ===");
            
            Console.Write("Введите телефон клиента: ");
            string phone = Console.ReadLine();
            
            Client client = manager.FindClientByPhone(phone);
            if (client == null)
            {
                Console.WriteLine("❌ Клиент не найден");
                return;
            }
            
            Console.Write("Упражнение: ");
            string exercise = Console.ReadLine();
            
            Console.Write("Вес (кг): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal weight))
            {
                weight = 0;
            }
            
            Console.Write("Количество повторений: ");
            if (!int.TryParse(Console.ReadLine(), out int reps))
            {
                reps = 10;
            }
            
            Console.Write("Количество подходов: ");
            if (!int.TryParse(Console.ReadLine(), out int sets))
            {
                sets = 3;
            }
            
            client.AddTrainingRecord(exercise, weight, reps, sets);
            Console.WriteLine("✅ Запись добавлена!");
        }
    }
}