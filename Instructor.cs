using System;
using System.Collections.Generic;
using System.Linq;

namespace FitnessCenter
{
    public class Instructor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public string Phone { get; set; }
        public DateTime HireDate { get; set; }
        
        // TODO 1: Добавлены свойства
        public string Qualifications { get; set; }  // "персональный тренер, диетолог"
        public string WorkSchedule { get; set; }    // "10:00-20:00"
        
        private List<TrainingSession> sessions = new List<TrainingSession>();
        private List<Client> clients = new List<Client>();
        
        public class TrainingSession
        {
            public Client Client { get; set; }
            public DateTime SessionDate { get; set; }
            public int DurationMinutes { get; set; }
            public string FocusArea { get; set; }
            public string Notes { get; set; }
        }
        
        public Instructor(int id, string name, string specialization, string phone, 
                         string qualifications, string workSchedule)
        {
            Id = id;
            FullName = name;
            Specialization = specialization;
            Phone = phone;
            HireDate = DateTime.Now;
            
            // TODO 1: Сохраняем квалификации и график
            Qualifications = qualifications;
            WorkSchedule = workSchedule;
        }
        
        // TODO 2: Добавить персональную тренировку
        public bool AddPersonalSession(Client client, DateTime sessionTime, int duration, string focus)
        {
            // Проверяем доступность
            if (!IsAvailable(sessionTime, duration))
            {
                Console.WriteLine("Тренер не доступен в указанное время.");
                return false;
            }
            
            // Создаем сессию
            TrainingSession session = new TrainingSession
            {
                Client = client,
                SessionDate = sessionTime,
                DurationMinutes = duration,
                FocusArea = focus,
                Notes = ""
            };
            
            sessions.Add(session);
            
            // Добавляем клиента, если его нет
            if (!clients.Contains(client))
                clients.Add(client);
                
            return true;
        }
        
        // TODO 2: Проверить доступность
        public bool IsAvailable(DateTime checkTime, int durationMinutes)
        {
            // Проверяем пересечение с существующими сессиями
            foreach (var session in sessions)
            {
                DateTime sessionStart = session.SessionDate;
                DateTime sessionEnd = sessionStart.AddMinutes(session.DurationMinutes);
                DateTime checkEnd = checkTime.AddMinutes(durationMinutes);
                
                // Проверяем пересечение временных интервалов
                if (checkTime < sessionEnd && checkEnd > sessionStart)
                    return false;
            }
            
            // TODO: Здесь можно добавить проверку WorkSchedule
            return true;
        }
        
        // TODO 3: Создать тренировочную программу
        public string CreateTrainingProgram(Client client, string goal)
        {
            string program = $"Тренировочная программа для {client.FullName}:\n";
            program += $"Цель: {goal}\n";
            program += $"Специализация тренера: {Specialization}\n\n";
            
            switch (goal.ToLower())
            {
                case "похудение":
                    program += "День 1 (Кардио): Бег 30 мин + Эллипс 20 мин\n";
                    program += "День 2 (Силовой): Круговая тренировка - 3 круга\n";
                    program += "   - Приседания 15 раз\n";
                    program += "   - Жим гантелей 12 раз\n";
                    program += "   - Тяга горизонтальная 15 раз\n";
                    program += "День 3 (Интервальный): HIIT 20 мин\n";
                    program += "Кардио в начале каждой тренировки\n";
                    break;
                    
                case "набор массы":
                    program += "База: 3 подхода по 8-10 повторений\n";
                    program += "День 1 (Грудь+Трицепс): Жим лежа, Жим гантелей, Французский жим\n";
                    program += "День 2 (Спина+Бицепс): Тяга штанги, Подтягивания, Подъем штанги на бицепс\n";
                    program += "День 3 (Ноги+Плечи): Приседания, Жим ногами, Армейский жим\n";
                    program += "Питание: профицит 300-500 ккал\n";
                    break;
                    
                case "поддержание":
                    program += "Сбалансированная программа:\n";
                    program += "- Силовая: 2 раза в неделю\n";
                    program += "- Кардио: 2 раза в неделю\n";
                    program += "- Растяжка: после каждой тренировки\n";
                    break;
                    
                default:
                    program += "Индивидуальная программа по запросу\n";
                    break;
            }
            
            program += $"\nТренер: {FullName}";
            return program;
        }
        
        // TODO 3: Статистика тренера
        public (int totalSessions, int activeClients, double avgSessionDuration) GetInstructorStats()
        {
            int totalSessions = sessions.Count;
            
            // Активные клиенты (тренировки за последние 30 дней)
            var monthAgo = DateTime.Now.AddDays(-30);
            var active = sessions
                .Where(s => s.SessionDate >= monthAgo)
                .Select(s => s.Client)
                .Distinct()
                .Count();
            
            double avgDuration = sessions.Count > 0 
                ? sessions.Average(s => s.DurationMinutes) 
                : 0;
            
            return (totalSessions, active, Math.Round(avgDuration, 0));
        }
        
        // TODO 1: Добавить квалификацию
        public void AddQualification(string qualification)
        {
            if (string.IsNullOrEmpty(Qualifications))
            {
                Qualifications = qualification;
            }
            else if (!Qualifications.Contains(qualification))
            {
                Qualifications += ", " + qualification;
            }
        }
        
        // TODO 2: Расписание на день
        public List<TrainingSession> GetDailySchedule(DateTime date)
        {
            return sessions
                .Where(s => s.SessionDate.Date == date.Date)
                .OrderBy(s => s.SessionDate)
                .ToList();
        }
        
        public void ShowInstructorInfo()
        {
            Console.WriteLine($"Тренер: {FullName}");
            Console.WriteLine($"Специализация: {Specialization}");
            Console.WriteLine($"Телефон: {Phone}");
            Console.WriteLine($"Дата найма: {HireDate:dd.MM.yyyy}");
            Console.WriteLine($"Стаж: {(DateTime.Now - HireDate).Days / 365} лет");
            // TODO 1: Выводим квалификации
            Console.WriteLine($"Квалификации: {Qualifications}");
            Console.WriteLine($"График работы: {WorkSchedule}");
            
            var stats = GetInstructorStats();
            Console.WriteLine($"\nСтатистика:");
            Console.WriteLine($"  Всего сессий: {stats.totalSessions}");
            Console.WriteLine($"  Активных клиентов: {stats.activeClients}");
            Console.WriteLine($"  Средняя продолжительность: {stats.avgSessionDuration} мин.");
            
            Console.WriteLine($"\nРасписание на сегодня ({DateTime.Today:dd.MM}):");
            var todaySchedule = GetDailySchedule(DateTime.Today);
            if (todaySchedule.Count > 0)
            {
                foreach (var session in todaySchedule)
                {
                    Console.WriteLine($"  {session.SessionDate:HH:mm} - {session.Client.FullName} ({session.FocusArea})");
                }
            }
            else
            {
                Console.WriteLine("  Сеансов нет");
            }
        }
    }
}