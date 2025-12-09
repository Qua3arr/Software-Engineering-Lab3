using System;

namespace Mediator_Pattern
{
    //Абстрактный посредник
    public abstract class Mediator
    {
        public abstract void Notify(Colleague sender, string message);
    }

    //Абстрактный класс коллеги
    public abstract class Colleague
    {
        protected Mediator mediator;

        public Colleague(Mediator mediator)
        {
            this.mediator = mediator;
        }

        public abstract void Send(string message);
        public abstract void Receive(string message);
    }

    //Конкретный посредник
    public class OrderMediator : Mediator
    {
        public Client Client { get; set; }
        public Manager Manager { get; set; }
        public Warehouse Warehouse { get; set; }

        public override void Notify(Colleague sender, string message)
        {
            Console.WriteLine($"\nПосредник получает сообщение от {sender.GetType().Name}: {message}");
            Console.WriteLine("Посредник перенаправляет сообщение...");

            if (sender is Client)
            {
                Manager.Receive(message);
            }
            else if (sender is Manager)
            {
                if (message.Contains("подтвердить"))
                    Client.Receive(message);
                else if (message.Contains("отправить"))
                    Warehouse.Receive(message);
            }
            else if (sender is Warehouse)
            {
                Manager.Receive(message);
            }
        }
    }

    //Конкретные классы коллег
    //Клиент
    public class Client : Colleague
    {
        public Client(Mediator mediator) : base(mediator) { }

        public override void Send(string message)
        {
            Console.WriteLine($"Клиент отправляет: {message}");
            mediator.Notify(this, message);
        }

        public override void Receive(string message)
        {
            Console.WriteLine($"Клиент получает: {message}");

            if (message.Contains("готов"))
            {
                Console.WriteLine("Клиент: Заказ получен, спасибо!");
            }
        }
    }

    //Менеджер
    public class Manager : Colleague
    {
        public Manager(Mediator mediator) : base(mediator) { }

        public override void Send(string message)
        {
            Console.WriteLine($"Менеджер отправляет: {message}");
            mediator.Notify(this, message);
        }

        public override void Receive(string message)
        {
            Console.WriteLine($"Менеджер получает: {message}");

            if (message.Contains("оформить заказ"))
            {
                Console.WriteLine("Менеджер: Заказ принят в обработку!");
                Send("Заказ подтвержден и отправлен на склад");
            }
            else if (message.Contains("товар упакован"))
            {
                Console.WriteLine("Менеджер: Отлично! Уведомляю клиента.");
                Send("Заказ готов к выдаче");
            }
        }
    }

    //Склад
    public class Warehouse : Colleague
    {
        public Warehouse(Mediator mediator) : base(mediator) { }

        public override void Send(string message)
        {
            Console.WriteLine($"Склад отправляет: {message}");
            mediator.Notify(this, message);
        }

        public override void Receive(string message)
        {
            Console.WriteLine($"Склад получает: {message}");

            if (message.Contains("отправлен на склад"))
            {
                Console.WriteLine("Склад: Начинаю комплектацию заказа...");
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Склад: Товар собран и упакован!");
                Send("Товар упакован и готов к отправке");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система управления заказами с паттерном Mediator ===\n");

            //Создаем посредника
            OrderMediator mediator = new OrderMediator();

            //Создаем компоненты системы
            Client client = new Client(mediator);
            Manager manager = new Manager(mediator);
            Warehouse warehouse = new Warehouse(mediator);

            //Регистрируем компоненты у посредника
            mediator.Client = client;
            mediator.Manager = manager;
            mediator.Warehouse = warehouse;

            //Симуляция процесса заказа
            Console.WriteLine("1. Клиент оформляет заказ:");
            client.Send("Хочу оформить заказ на товар #12345");

            Console.WriteLine("\n\n2. Менеджер обрабатывает заказ:");
            //Менеджер автоматически получил уведомление через посредника

            Console.WriteLine("\n\n3. Склад обрабатывает заказ:");
            //Склад автоматически получил уведомление через посредника

            Console.WriteLine("\n\n4. Клиент получает уведомление:");
            //Клиент автоматически получил уведомление через посредника

            Console.WriteLine("\n\n=== Процесс завершен ===");
        }
    }
}
