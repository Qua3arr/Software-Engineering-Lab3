using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template_Method_Pattern
{
    internal class Program
    {
        //Класс заказа
        public class Order
        {
            public string Id { get; set; }
            public string CustomerName { get; set; }
            public decimal TotalAmount { get; set; }
            public string DeliveryMethod { get; set; }
            public string PaymentMethod { get; set; }
            public DateTime OrderDate { get; set; }

            public Order(string customerName, decimal totalAmount)
            {
                Id = Guid.NewGuid().ToString();
                CustomerName = customerName;
                TotalAmount = totalAmount;
                OrderDate = DateTime.Now;
            }

            public void DisplayOrderInfo()
            {
                Console.WriteLine($"Заказ #{Id}");
                Console.WriteLine($"Клиент: {CustomerName}");
                Console.WriteLine($"Сумма: {TotalAmount}");
                Console.WriteLine($"Дата: {OrderDate:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"Способ доставки: {DeliveryMethod}");
                Console.WriteLine($"Способ оплаты: {PaymentMethod}");
            }
        }

        //Абстрактный класс, определяющий шаблонный метод
        public abstract class OrderProcessing
        {
            //Шаблонный метод - определяет общий алгоритм
            public void ProcessOrder(Order order)
            {
                Console.WriteLine("=== Начало обработки заказа ===");

                SelectProducts(order);
                ValidateOrder(order);
                ApplyDiscounts(order);
                ProcessPayment(order);
                ArrangeDelivery(order);
                SendConfirmation(order);

                Console.WriteLine("=== Обработка заказа завершена ===\n");
            }

            protected virtual void SelectProducts(Order order)
            {
                Console.WriteLine("1. Товары выбраны в корзину");
            }

            protected virtual void ValidateOrder(Order order)
            {
                Console.WriteLine("2. Заказ проверен на корректность");
            }

            protected virtual void ApplyDiscounts(Order order)
            {
                Console.WriteLine("3. Применены доступные скидки");
            }

            protected virtual void ProcessPayment(Order order)
            {
                Console.WriteLine($"4. Оплата {order.TotalAmount} проведена через {order.PaymentMethod}");
            }

            protected abstract void ArrangeDelivery(Order order);

            protected virtual void SendConfirmation(Order order)
            {
                Console.WriteLine("6. Отправлено стандартное подтверждение заказа");
            }
        }

        //Конкретная реализация для стандартной доставки
        public class StandardOrderProcessing : OrderProcessing
        {
            protected override void ArrangeDelivery(Order order)
            {
                order.DeliveryMethod = "Стандартная доставка (5-7 дней)";
                order.PaymentMethod = "Банковская карта";
                Console.WriteLine("5. Оформлена стандартная доставка");
                Console.WriteLine("   • Срок доставки: 5-7 рабочих дней");
                Console.WriteLine("   • Стоимость доставки: 299 руб.");
            }

            protected override void ApplyDiscounts(Order order)
            {
                base.ApplyDiscounts(order);
                // Дополнительная скидка для стандартных заказов
                if (order.TotalAmount > 5000)
                {
                    order.TotalAmount *= 0.95m;
                    Console.WriteLine("   • Применена дополнительная скидка 5% для заказа свыше 5000 руб.");
                }
            }
        }

        //Конкретная реализация для экспресс-доставки
        public class ExpressOrderProcessing : OrderProcessing
        {
            protected override void ArrangeDelivery(Order order)
            {
                order.DeliveryMethod = "Экспресс-доставка (1-2 дня)";
                order.PaymentMethod = "Онлайн-оплата";
                decimal deliveryCost = 799m;
                order.TotalAmount += deliveryCost;
                Console.WriteLine("5. Оформлена экспресс-доставка");
                Console.WriteLine($"   • Срок доставки: 1-2 рабочих дня");
                Console.WriteLine($"   • Стоимость доставки: {deliveryCost} руб.");
            }

            protected override void SendConfirmation(Order order)
            {
                Console.WriteLine("6. Отправлено срочное подтверждение заказа");
                Console.WriteLine("   • SMS уведомление клиенту");
                Console.WriteLine("   • Экспресс-уведомление менеджеру");
            }

            protected override void ValidateOrder(Order order)
            {
                base.ValidateOrder(order);
                Console.WriteLine("   • Дополнительная проверка доступности товаров для срочной доставки");
            }
        }

        //Класс для обработки заказа
        public class OrderProcessor
        {
            private OrderProcessing _processingStrategy;

            public OrderProcessor(OrderProcessing processingStrategy)
            {
                _processingStrategy = processingStrategy;
            }

            public void SetProcessingStrategy(OrderProcessing processingStrategy)
            {
                _processingStrategy = processingStrategy;
            }

            public void Process(Order order)
            {
                _processingStrategy.ProcessOrder(order);
                order.DisplayOrderInfo();
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("=== Интернет-магазин: Система обработки заказов ===\n");

            //Создаём заказы
            var order1 = new Order("Крутой Чел", 4500m);
            var order2 = new Order("Павел Дуров", 7500m);
            var order3 = new Order("Серёга", 3200m);

            //Создаём процессоры с разными стратегиями
            var standardProcessor = new OrderProcessor(new StandardOrderProcessing());
            var expressProcessor = new OrderProcessor(new ExpressOrderProcessing());

            Console.WriteLine("1. Обработка заказа со СТАНДАРТНОЙ доставкой:");
            standardProcessor.Process(order1);

            Console.WriteLine("\n2. Обработка заказа с ЭКСПРЕСС доставкой:");
            expressProcessor.Process(order2);

            Console.WriteLine("\n3. Смена стратегии обработки:");
            var processor = new OrderProcessor(new StandardOrderProcessing());
            processor.Process(order3);

            Console.WriteLine("Меняем на экспресс-доставку...");
            processor.SetProcessingStrategy(new ExpressOrderProcessing());
            processor.Process(order3);

            //Пример пакетной обработки
            Console.WriteLine("\n4. Пакетная обработка заказов:");
            var orders = new[]
            {
                new Order("Трамп", 6000m),
                new Order("Санёк", 2500m)
            };

            OrderProcessing[] processors = { new StandardOrderProcessing(), new ExpressOrderProcessing() };

            for (int i = 0; i < orders.Length; i++)
            {
                Console.WriteLine($"\nЗаказ {i + 1}:");
                var selectedProcessor = orders[i].TotalAmount > 5000 ? processors[1] : processors[0];
                selectedProcessor.ProcessOrder(orders[i]);
                orders[i].DisplayOrderInfo();
            }
        }
    }
}
