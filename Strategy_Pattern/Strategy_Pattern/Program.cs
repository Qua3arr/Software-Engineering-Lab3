using System;


namespace Strategy_Pattern
{
    // Абстрактный класс стратегии оплаты
    public abstract class PaymentStrategy
    {
        public abstract void ProcessPayment(decimal amount);
    }

    // Конкретные стратегии оплаты
    // Оплата картой
    public class CashlessPayment : PaymentStrategy
    {
        public override void ProcessPayment(decimal amount)
        {
            Console.WriteLine($"Обработка безналичной оплаты на сумму {amount}");
            Console.WriteLine("Списание средств с банковской карты...");
            Console.WriteLine("Платеж успешно проведен!\n");
        }
    }

    // Наличный расчет
    public class CashPayment : PaymentStrategy
    {
        public override void ProcessPayment(decimal amount)
        {
            Console.WriteLine($"Обработка наличного расчета на сумму {amount}");
            Console.WriteLine("Ожидание передачи наличных...");
            Console.WriteLine("Наличные получены!\n");
        }
    }

    // Перевод при получении
    public class CashOnDeliveryPayment : PaymentStrategy
    {
        public override void ProcessPayment(decimal amount)
        {
            Console.WriteLine($"Оформлен перевод при получении на сумму {amount}");
            Console.WriteLine("Товар будет передан курьеру для доставки с оплатой при получении");
            Console.WriteLine("Клиент оплатит при вручении заказа\n");
        }
    }

    // Класс заказа, использующий стратегию
    public class Order
    {
        private PaymentStrategy _paymentStrategy;
        public string OrderId { get; }
        public decimal TotalAmount { get; }
        public string CustomerName { get; }

        public Order(string orderId, decimal totalAmount, string customerName)
        {
            OrderId = orderId;
            TotalAmount = totalAmount;
            CustomerName = customerName;
        }

        // Метод для установки стратегии оплаты (можно менять динамически)
        public void SetPaymentStrategy(PaymentStrategy paymentStrategy)
        {
            _paymentStrategy = paymentStrategy;
            Console.WriteLine($"Для заказа {OrderId} выбран способ оплаты: {paymentStrategy.GetType().Name}");
        }

        // Обработка платежа с использованием текущей стратегии
        public void ProcessPayment()
        {
            if (_paymentStrategy == null)
            {
                Console.WriteLine("Ошибка: не выбран способ оплаты!");
                return;
            }

            Console.WriteLine($"\nОбработка заказа #{OrderId}");
            Console.WriteLine($"Клиент: {CustomerName}");
            Console.WriteLine($"Сумма к оплате: {TotalAmount}");

            _paymentStrategy.ProcessPayment(TotalAmount);

            Console.WriteLine($"Заказ #{OrderId} успешно оформлен!");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система оплаты интернет-магазина ===\n");

            // Создаем заказ
            var order = new Order("ORD-2025-001", 12500.50m, "Гордон Фримен");

            // Выбираем и применяем разные стратегии оплаты
            Console.WriteLine("\n--- Сценарий 1: Безналичная оплата ---");
            order.SetPaymentStrategy(new CashlessPayment());
            order.ProcessPayment();

            Console.WriteLine("\n--- Сценарий 2: Наличный расчет ---");
            order.SetPaymentStrategy(new CashPayment());
            order.ProcessPayment();

            Console.WriteLine("\n--- Сценарий 3: Перевод при получении ---");
            order.SetPaymentStrategy(new CashOnDeliveryPayment());
            order.ProcessPayment();

            // Демонстрация динамической смены стратегии
            Console.WriteLine("\n=== Демонстрация динамической смены стратегии ===");
            var newOrder = new Order("ORD-2025-002", 8500.00m, "Гейб Ньюэлл");

            newOrder.SetPaymentStrategy(new CashlessPayment());
 
            newOrder.SetPaymentStrategy(new CashOnDeliveryPayment());
            newOrder.ProcessPayment();
        }
    }
}
/*
        ========================================================================
        ПОШАГОВАЯ ИНСТРУКЦИЯ ПО ДОБАВЛЕНИЮ НОВОГО МЕТОДА ОПЛАТЫ
        (например, криптовалюты)
        ========================================================================
        
        ШАГ 1: Создать новый класс стратегии
           - Создайте новый класс в этом же файле (или в отдельном файле)
           - Наследуйте его от абстрактного класса PaymentStrategy
           - Реализуйте обязательный метод ProcessPayment(decimal amount)
        
        Пример:
        
        public class CryptocurrencyPayment : PaymentStrategy
        {
            private string _cryptoType;
            
            public CryptocurrencyPayment(string cryptoType = "Bitcoin")
            {
                _cryptoType = cryptoType;
            }
            
            public override void ProcessPayment(decimal amount)
            {
                Console.WriteLine($"Обработка оплаты {_cryptoType} на сумму {amount}");
                // Логика работы с криптовалютой...
            }
        }
        
        
        ШАГ 2: Использовать новую стратегию в коде
           - Создайте экземпляр новой стратегии
           - Установите ее для заказа с помощью метода SetPaymentStrategy()
           - Вызовите ProcessPayment() для обработки платежа
        
        Пример:

        var order = new Order("ORD-001", 5000.00m, "Paul Du Rove");
        order.SetPaymentStrategy(new CryptocurrencyPayment("Toncoin"));
        order.ProcessPayment();

       */