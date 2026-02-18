using System;
using System.Linq;

namespace FitnessCenter
{
    public class Membership
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // TODO 1: Добавлены свойства
        public int VisitsAllowed { get; set; }  // 0 = безлимит
        public string ServicesIncluded { get; set; }  // "зал,бассейн,групповые"
        
        public Membership(int id, string type, decimal price, DateTime startDate, 
                         int visitsAllowed, string servicesIncluded)
        {
            Id = id;
            Type = type;
            StartDate = startDate;
            
            // TODO 2: Проверка цены
            Price = price < 0 ? 500 : price;
            
            // TODO 2: Расчет даты окончания
            switch (type.ToLower())
            {
                case "разовый":
                    EndDate = startDate.Date.AddDays(1).AddSeconds(-1); // Конец дня
                    break;
                case "месячный":
                    EndDate = startDate.AddDays(30);
                    break;
                case "годовой":
                    EndDate = startDate.AddDays(365);
                    break;
                case "студенческий":
                    EndDate = startDate.AddDays(30);
                    break;
                default:
                    EndDate = startDate.AddDays(30); // По умолчанию месяц
                    break;
            }
            
            // TODO 1: Сохраняем параметры
            VisitsAllowed = visitsAllowed;
            ServicesIncluded = servicesIncluded;
        }
        
        // TODO 3: Проверка действительности
        public bool IsValid()
        {
            DateTime now = DateTime.Now;
            
            // Проверка даты
            if (now < StartDate || now > EndDate)
                return false;
            
            // Проверка количества посещений
            if (VisitsAllowed < 0)
                return false;
                
            // Если VisitsAllowed == 0 (безлимит) - всегда true при действующей дате
            return true;
        }
        
        // TODO 1: Использовать посещение
        public bool UseVisit()
        {
            if (!IsValid())
                return false;
                
            if (VisitsAllowed > 0)
            {
                VisitsAllowed--;
                return true;
            }
            
            // VisitsAllowed == 0 (безлимит)
            return true;
        }
        
        // TODO 1: Проверка включения услуги
        public bool IncludesService(string service)
        {
            if (string.IsNullOrEmpty(ServicesIncluded) || string.IsNullOrEmpty(service))
                return false;
                
            var services = ServicesIncluded.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return services.Any(s => s.Trim().ToLower() == service.ToLower());
        }
        
        // TODO 2: Оставшиеся дни
        public int GetDaysRemaining()
        {
            DateTime now = DateTime.Now;
            
            if (now > EndDate)
                return 0;
                
            return (EndDate - now).Days;
        }
        
        public override string ToString()
        {
            return $"Абонемент #{Id}: {Type} - {Price} руб. (до {EndDate:dd.MM.yyyy})";
        }
    }
}