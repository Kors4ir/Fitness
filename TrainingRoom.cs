using System;

namespace FitnessCenter
{
    public class TrainingRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string WorkingHours { get; set; }
        public string Equipment { get; set; }
        
        private int currentVisitors = 0;
        
        public TrainingRoom(int id, string name, int capacity, string workingHours, string equipment)
        {
            Id = id;
            Name = name;
            Capacity = capacity;
            WorkingHours = workingHours;
            Equipment = equipment;
        }
        
        public bool RegisterClientEntry()
        {
            if (currentVisitors < Capacity)
            {
                currentVisitors++;
                return true;
            }
            return false;
        }
        
        public void RegisterClientExit()
        {
            if (currentVisitors > 0)
                currentVisitors--;
        }
        
        public int GetCurrentLoad()
        {
            return currentVisitors;
        }
        
        public int GetLoadPercentage()
        {
            if (Capacity == 0) return 0;
            return (int)((double)currentVisitors / Capacity * 100);
        }
        
        public void ShowRoomInfo()
        {
            Console.WriteLine($"Зал: {Name}");
            Console.WriteLine($"  ID: {Id}, Вместимость: {Capacity} чел.");
            Console.WriteLine($"  Работает: {WorkingHours}");
            Console.WriteLine($"  Оборудование: {Equipment}");
            Console.WriteLine($"  Текущая загрузка: {currentVisitors}/{Capacity} ({GetLoadPercentage()}%)");
        }
    }
}