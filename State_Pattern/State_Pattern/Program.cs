using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace State_Pattern
{
    //Абстрактный класс состояния заказа
    public abstract class OrderState
    {
        public abstract void ProcessOrder(Order order);
        public abstract string GetStatus();
        public virtual void CancelOrder(Order order)
        {
            throw new InvalidOperationException($"Невозможно отменить заказ в состоянии: {GetStatus()}");
        }
    }

    //Конкретные состояния
    //Состояние: Новый заказ
    public class NewOrderState : OrderState
    {
        public override void ProcessOrder(Order order)
        {
            Console.WriteLine("Заказ взят в обработку...");
            order.SetState(new ProcessingOrderState());
        }

        public override string GetStatus()
        {
            return "Новый";
        }

        public override void CancelOrder(Order order)
        {
            Console.WriteLine("Заказ отменен.");
            order.SetState(new CancelledOrderState());
        }
    }

    //Состояние: В обработке
    public class ProcessingOrderState : OrderState
    {
        public override void ProcessOrder(Order order)
        {
            Console.WriteLine("Заказ отправляется...");
            order.SetState(new ShippedOrderState());
        }

        public override string GetStatus()
        {
            return "В обработке";
        }

        public override void CancelOrder(Order order)
        {
            Console.WriteLine("Заказ отменен на этапе обработки.");
            order.SetState(new CancelledOrderState());
        }
    }

    //Состояние: Отправлен
    public class ShippedOrderState : OrderState
    {
        public override void ProcessOrder(Order order)
        {
            Console.WriteLine("Заказ доставлен!");
            order.SetState(new DeliveredOrderState());
        }

        public override string GetStatus()
        {
            return "Отправлен";
        }

        public override void CancelOrder(Order order)
        {
            Console.WriteLine("Невозможно отменить отправленный заказ. Обратитесь в службу поддержки.");
        }
    }

    //Состояние: Доставлен
    public class DeliveredOrderState : OrderState
    {
        public override void ProcessOrder(Order order)
        {
            Console.WriteLine("Заказ уже доставлен. Никаких дальнейших действий не требуется.");
        }

        public override string GetStatus()
        {
            return "Доставлен";
        }

        public override void CancelOrder(Order order)
        {
            Console.WriteLine("Невозможно отменить доставленный заказ.");
        }
    }

    //Состояние: Отменен
    public class CancelledOrderState : OrderState
    {
        public override void ProcessOrder(Order order)
        {
            Console.WriteLine("Невозможно обработать отмененный заказ.");
        }

        public override string GetStatus()
        {
            return "Отменен";
        }
    }

    //Класс заказа
    public class Order
    {
        private OrderState _currentState;
        public string OrderNumber { get; }
        public DateTime CreatedDate { get; }

        public Order(string orderNumber)
        {
            OrderNumber = orderNumber;
            CreatedDate = DateTime.Now;
            _currentState = new NewOrderState();
        }

        public void SetState(OrderState state)
        {
            _currentState = state;
        }

        public void Process()
        {
            Console.WriteLine($"\nОбработка заказа #{OrderNumber}:");
            _currentState.ProcessOrder(this);
        }

        public void Cancel()
        {
            Console.WriteLine($"\nПопытка отмены заказа #{OrderNumber}:");
            _currentState.CancelOrder(this);
        }

        public string GetStatus()
        {
            return _currentState.GetStatus();
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"\nЗаказ #{OrderNumber}");
            Console.WriteLine($"Дата создания: {CreatedDate:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Текущий статус: {GetStatus()}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система управления состоянием заказа ===\n");

            //Создаем новый заказ
            Order order = new Order("ORD-2025-001");
            order.DisplayInfo();

            //Обрабатываем заказ
            order.Process();
            order.DisplayInfo();

            //Продолжаем обработку
            order.Process();
            order.DisplayInfo();

            //Завершаем доставку
            order.Process();
            order.DisplayInfo();

            //Пытаемся обработать уже доставленный заказ
            order.Process();

            Console.WriteLine("\n--- Создаем новый заказ для демонстрации отмены ---\n");

            //Создаем другой заказ
            Order order2 = new Order("ORD-2025-002");
            order2.DisplayInfo();

            //Отменяем новый заказ
            order2.Cancel();
            order2.DisplayInfo();

            //Пытаемся обработать отмененный заказ
            order2.Process();

            Console.WriteLine("\n--- Создаем третий заказ ---\n");

            Order order3 = new Order("ORD-2025-003");
            order3.DisplayInfo();

            //Обрабатываем
            order3.Process();
            order3.DisplayInfo();

            //Пытаемся отменить заказ в обработке
            order3.Cancel();
            order3.DisplayInfo();

            Console.WriteLine("\n--- Попытка отменить отправленный заказ ---\n");

            Order order4 = new Order("ORD-2025-004");
            order4.Process(); //В обработку
            order4.Process(); //Отправлен
            order4.DisplayInfo();
            order4.Cancel(); //Попытка отменить отправленный заказ
        }
    }
}
