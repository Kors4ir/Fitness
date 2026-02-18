using System;
using System.Collections.Generic;
using System.Linq;

namespace FitnessCenter
{
    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        
        // TODO 1: Добавлены свойства для роста, веса и цели
        public decimal Height { get; set; }      // Рост в см
        public decimal Weight { get; set; }      // Вес в кг
        public string FitnessGoal { get; set; }  // Цель: похудение, набор массы, поддержание
        
        private Membership currentMembership;
        private List<Visit> visitHistory = new List<Visit>();
        private List<TrainingRecord> trainingHistory = new List<TrainingRecord>();
        
        public class Visit
        {
            public DateTime VisitDate { get; set; }
            public string ServiceUsed { get; set; }
            public int DurationMinutes { get; set; }
        }
        
        public class TrainingRecord
        {
            public DateTime TrainingDate { get; set; }
            public string Exercise { get; set; }
            public decimal Weight { get; set; }
            public int Repetitions { get; set; }
            public int Sets { get; set; }
        }
        
        public Client(int id, string name, string phone, DateTime birthDate, 
                     decimal height, decimal weight, string goal)
        {
            Id = id;
            FullName = name;
            Phone = phone;
            BirthDate = birthDate;
            RegistrationDate = DateTime.Now;
            
            // TODO 1: Сохраняем рост, вес и цель
            Height = height;
            Weight = weight;
            FitnessGoal = goal;
        }
        
        // TODO 2: Добавление абонемента
        public bool AddMembership(Membership membership)
        {
            // Проверяем, нет ли действующего абонемента
            if (currentMembership != null && currentMembership.IsValid())
            {
                Console.WriteLine("У клиента уже есть действующий абонемент.");
                return false;
            }
            
            currentMembership = membership;
            return true;
        }
        
        // TODO 2: Регистрация посещения
        public bool RegisterVisit(string service, int duration)
        {
            // Проверяем наличие действующего абонемента
            if (currentMembership == null || !currentMembership.IsValid())
            {
                Console.WriteLine("Абонемент недействителен или отсутствует.");
                return false;
            }
            
            // Проверяем, включает ли абонемент услугу
            if (!currentMembership.IncludesService(service))
            {
                Console.WriteLine($"Абонемент не включает услугу '{service}'.");
                return false;
            }
            
            // Используем посещение
            if (!currentMembership.UseVisit())
            {
                Console.WriteLine("Нет доступных посещений.");
                return false;
            }
            
            // Создаем запись о посещении
            Visit visit = new Visit
            {
                VisitDate = DateTime.Now,
                ServiceUsed = service,
                DurationMinutes = duration
            };
            visitHistory.Add(visit);
            
            return true;
        }
        
        // TODO 3: Расчет ИМТ
        public decimal CalculateBMI()
        {
            if (Height <= 0 || Weight <= 0)
                return 0;
                
            // ИМТ = вес(кг) / (рост(м))^2
            decimal heightInMeters = Height / 100;
            return Math.Round(Weight / (heightInMeters * heightInMeters), 2);
        }
        
        // TODO 3: Рекомендация по ИМТ
        public string GetBMIRecommendation()
        {
            decimal bmi = CalculateBMI();
            
            if (bmi < 18.5m)
                return "Недостаточный вес. Рекомендуется: усиленное питание, силовые тренировки для набора массы.";
            else if (bmi >= 18.5m && bmi < 25m)
                return "Нормальный вес. Рекомендуется: поддержание формы, сбалансированные тренировки.";
            else if (bmi >= 25m && bmi < 30m)
                return "Избыточный вес. Рекомендуется: кардиотренировки, коррекция питания, снижение веса.";
            else
                return "Ожирение. Рекомендуется: консультация врача, умеренные кардионагрузки, строгая диета.";
        }
        
        // TODO 2: Добавление записи о тренировке
        public void AddTrainingRecord(string exercise, decimal weight, int reps, int sets)
        {
            TrainingRecord record = new TrainingRecord
            {
                TrainingDate = DateTime.Now,
                Exercise = exercise,
                Weight = weight,
                Repetitions = reps,
                Sets = sets
            };
            trainingHistory.Add(record);
            Console.WriteLine($"Запись о тренировке добавлена: {exercise} {weight}кг x{reps} x{sets}");
        }
        
        // TODO 1: Расчет возраста
        public int CalculateAge()
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;
            if (BirthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
        
        // TODO 3: Статистика посещений
        public (int totalVisits, double avgDuration, string mostVisitedService) GetVisitStats()
        {
            int total = visitHistory.Count;
            
            if (total == 0)
                return (0, 0, "нет посещений");
            
            double avgDuration = visitHistory.Average(v => v.DurationMinutes);
            
            var serviceGroups = visitHistory
                .GroupBy(v => v.ServiceUsed)
                .Select(g => new { Service = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();
            
            string mostVisited = serviceGroups?.Service ?? "нет данных";
            
            return (total, Math.Round(avgDuration, 0), mostVisited);
        }
        
        public void ShowClientInfo()
        {
            Console.WriteLine($"Клиент: {FullName}");
            Console.WriteLine($"Телефон: {Phone}");
            Console.WriteLine($"Дата рождения: {BirthDate:dd.MM.yyyy}");
            Console.WriteLine($"Возраст: {CalculateAge()} лет");
            // TODO 1: Выводим рост, вес и цель
            Console.WriteLine($"Рост: {Height} см, Вес: {Weight} кг, ИМТ: {CalculateBMI():F1}");
            Console.WriteLine($"Цель: {FitnessGoal}");
            
            if (currentMembership != null)
            {
                Console.WriteLine($"\nАбонемент: {currentMembership.Type}");
                Console.WriteLine($"Действует до: {currentMembership.EndDate:dd.MM.yyyy}");
                Console.WriteLine($"Осталось дней: {currentMembership.GetDaysRemaining()}");
                Console.WriteLine($"Осталось посещений: {(currentMembership.VisitsAllowed == 0 ? "безлимит" : currentMembership.VisitsAllowed.ToString())}");
                Console.WriteLine($"Статус: {(currentMembership.IsValid() ? "Действителен" : "Просрочен")}");
            }
            else
            {
                Console.WriteLine("\nАбонемент: отсутствует");
            }
            
            var stats = GetVisitStats();
            Console.WriteLine($"\nВсего посещений: {stats.totalVisits}");
            Console.WriteLine($"Средняя продолжительность: {stats.avgDuration:F0} мин.");
            Console.WriteLine($"Самая посещаемая услуга: {stats.mostVisitedService}");
            
            decimal bmi = CalculateBMI();
            if (bmi > 0)
            {
                Console.WriteLine($"ИМТ: {bmi:F1} - {GetBMIRecommendation()}");
            }
            
            if (trainingHistory.Count > 0)
            {
                Console.WriteLine($"\nПоследние тренировки:");
                var last3 = trainingHistory.OrderByDescending(t => t.TrainingDate).Take(3);
                foreach (var t in last3)
                {
                    Console.WriteLine($"  {t.TrainingDate:dd.MM}: {t.Exercise} {t.Weight}кг x{t.Repetitions} x{t.Sets}");
                }
            }
        }
    }
}