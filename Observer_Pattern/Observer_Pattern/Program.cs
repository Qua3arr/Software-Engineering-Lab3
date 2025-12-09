using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observer_Pattern
{
    //Интерфейс наблюдателя
    public interface IObserver
    {
        void Update(Order order);
    }

    //Интерфейс субъекта
    public interface ISubject
    {
        void AddObserver(IObserver observer);
        void RemoveObserver(IObserver observer);
        void NotifyObservers();
    }

    //Класс заказа
    public class Order : ISubject
    {
        private int _id;
        private string _status;
        private List<IObserver> _observers = new List<IObserver>();

        public int Id => _id;

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    NotifyObservers();
                }
            }
        }

        public Order(int id, string initialStatus = "Оформлен")
        {
            _id = id;
            _status = initialStatus;
        }

        public void AddObserver(IObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void RemoveObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var observer in _observers.ToArray())
            {
                observer.Update(this);
            }
        }
    }

    //Наблюдатель: Уведомление клиента
    public class ClientNotification : IObserver
    {
        private string _clientName;

        public ClientNotification(string clientName)
        {
            _clientName = clientName;
        }

        public void Update(Order order)
        {
            Console.WriteLine($"Клиент {_clientName}: Статус заказа #{order.Id} изменен на '{order.Status}'");
        }
    }

    //Наблюдатель: Уведомление менеджера
    public class ManagerNotification : IObserver
    {
        private string _managerName;

        public ManagerNotification(string managerName)
        {
            _managerName = managerName;
        }

        public void Update(Order order)
        {
            Console.WriteLine($"Менеджер {_managerName}: Получено обновление по заказу #{order.Id} - статус '{order.Status}'");

            if (order.Status == "Отправлен")
            {
                Console.WriteLine($"    Менеджер {_managerName}: Проверяю логистику...");
            }
        }
    }

    //Наблюдатель: Аналитическая система
    public class AnalyticsSystem : IObserver
    {
        private Dictionary<string, int> _statusStatistics = new Dictionary<string, int>();

        public void Update(Order order)
        {
            if (_statusStatistics.ContainsKey(order.Status))
            {
                _statusStatistics[order.Status]++;
            }
            else
            {
                _statusStatistics[order.Status] = 1;
            }

            Console.WriteLine($"Аналитическая система: Статистика обновлена. Статус '{order.Status}' встречался {_statusStatistics[order.Status]} раз(а)");

            if (order.Status == "Доставлен")
            {
                Console.WriteLine("    Анализ: Заказ доставлен, можно запросить отзыв у клиента");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            //Создаем заказ
            var order = new Order(12345, "Оформлен");

            //Создаем наблюдателей
            var client = new ClientNotification("Иван Иванов");
            var manager = new ManagerNotification("Петр Петров");
            var analytics = new AnalyticsSystem();

            //Подписываем наблюдателей на заказ
            order.AddObserver(client);
            order.AddObserver(manager);
            order.AddObserver(analytics);

            Console.WriteLine("=== Изменение статусов заказа ===");

            //Меняем статусы заказа
            order.Status = "В обработке";
            Console.WriteLine();

            order.Status = "Отправлен";
            Console.WriteLine();

            //Отписываем менеджера
            order.RemoveObserver(manager);

            order.Status = "Доставлен";
            Console.WriteLine();

            //Пытаемся добавить null-наблюдателя
            try
            {
                order.AddObserver(null);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
