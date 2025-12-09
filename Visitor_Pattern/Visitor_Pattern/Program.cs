using System;
using System.Collections.Generic;

namespace Visitor_Pattern
{
    //Базовый интерфейс для всех компонентов заказа
    public interface IOrderComponent
    {
        decimal GetPrice();
        void Accept(Visitor visitor);
    }

    //Абстрактный класс Посетителя
    public abstract class Visitor
    {
        public abstract void VisitProduct(Product product);
        public abstract void VisitBox(Box box);
    }

    //Класс Продукта
    public class Product : IOrderComponent
    {
        public string Name { get; }
        public decimal Price { get; }
        public double Weight { get; }

        public Product(string name, decimal price, double weight = 1.0)
        {
            Name = name;
            Price = price;
            Weight = weight;
        }

        public decimal GetPrice() => Price;

        public void Accept(Visitor visitor)
        {
            visitor.VisitProduct(this);
        }
    }

    //Класс Коробки (компоновщик)
    public class Box : IOrderComponent
    {
        private readonly List<IOrderComponent> _children = new List<IOrderComponent>();

        public string Name { get; }
        public double Weight { get; }

        public Box(string name, double weight = 0.5)
        {
            Name = name;
            Weight = weight;
        }

        public void Add(IOrderComponent component) => _children.Add(component);
        public void Remove(IOrderComponent component) => _children.Remove(component);

        public decimal GetPrice()
        {
            decimal total = 0;
            foreach (var child in _children)
            {
                total += child.GetPrice();
            }
            return total;
        }

        public void Accept(Visitor visitor)
        {
            foreach (var child in _children)
            {
                child.Accept(visitor);
            }
            visitor.VisitBox(this);
        }

        //Метод для получения общего веса (рекурсивно)
        public double GetTotalWeight()
        {
            double totalWeight = Weight;
            foreach (var child in _children)
            {
                if (child is Product product)
                {
                    totalWeight += product.Weight;
                }
                else if (child is Box box)
                {
                    totalWeight += box.GetTotalWeight();
                }
            }
            return totalWeight;
        }
    }

    //Посетитель для расчета стоимости доставки
    public class DeliveryCostCalculator : Visitor
    {
        private decimal _totalDeliveryCost = 0;

        private const decimal BaseDeliveryCostPerKg = 2.0m;
        private const decimal BoxPackagingFee = 1.5m;
        private const decimal ProductHandlingFee = 0.5m;

        public decimal TotalDeliveryCost => _totalDeliveryCost;

        public override void VisitProduct(Product product)
        {
            decimal deliveryCost = (decimal)product.Weight * BaseDeliveryCostPerKg + ProductHandlingFee;
            _totalDeliveryCost += deliveryCost;

            Console.WriteLine($"Доставка продукта '{product.Name}': ${deliveryCost:F2}");
        }

        public override void VisitBox(Box box)
        {
            decimal deliveryCost = (decimal)box.Weight * BaseDeliveryCostPerKg + BoxPackagingFee;
            _totalDeliveryCost += deliveryCost;

            Console.WriteLine($"Доставка коробки '{box.Name}': ${deliveryCost:F2}");
        }

        public void Reset()
        {
            _totalDeliveryCost = 0;
        }
    }

    //Посетитель для расчета налогов
    public class TaxCalculator : Visitor
    {
        private decimal _totalTax = 0;

        //Налоговые ставки
        private const decimal StandardTaxRate = 0.2m; // 20% стандартный налог
        private const decimal ReducedTaxRate = 0.1m; // 10% сниженный налог для электроники

        public decimal TotalTax => _totalTax;

        public override void VisitProduct(Product product)
        {
            decimal taxRate = IsElectronic(product) ? ReducedTaxRate : StandardTaxRate;
            decimal tax = product.Price * taxRate;
            _totalTax += tax;

            Console.WriteLine($"Налог на продукт '{product.Name}' ({taxRate * 100}%): ${tax:F2}");
        }

        public override void VisitBox(Box box)
        {
            decimal packagingTax = (decimal)box.Weight * 0.1m; // Небольшой налог на упаковку
            _totalTax += packagingTax;

            Console.WriteLine($"Налог на упаковку коробки '{box.Name}': ${packagingTax:F2}");
        }

        private bool IsElectronic(Product product)
        {
            string[] electronicKeywords = { "laptop", "phone", "tablet", "mouse", "keyboard", "headphones", "cable" };
            string name = product.Name.ToLower();

            foreach (var keyword in electronicKeywords)
            {
                if (name.Contains(keyword))
                    return true;
            }
            return false;
        }

        public void Reset()
        {
            _totalTax = 0;
        }
    }

    //Класс Заказа
    public class Order
    {
        private readonly List<IOrderComponent> _components = new List<IOrderComponent>();

        public void AddComponent(IOrderComponent component) => _components.Add(component);

        public decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (var component in _components)
            {
                total += component.GetPrice();
            }
            return total;
        }

        public void Accept(Visitor visitor)
        {
            foreach (var component in _components)
            {
                component.Accept(visitor);
            }
        }

        public void PrintOrderSummary()
        {
            Console.WriteLine("\n=== Сводка по заказу ===");

            decimal subtotal = CalculateTotal();
            Console.WriteLine($"Стоимость товаров: ${subtotal:F2}");

            var taxCalculator = new TaxCalculator();
            Accept(taxCalculator);
            Console.WriteLine($"Итого налог: ${taxCalculator.TotalTax:F2}");

            var deliveryCalculator = new DeliveryCostCalculator();
            Accept(deliveryCalculator);
            Console.WriteLine($"Стоимость доставки: ${deliveryCalculator.TotalDeliveryCost:F2}");

            decimal total = subtotal + taxCalculator.TotalTax + deliveryCalculator.TotalDeliveryCost;
            Console.WriteLine($"ОБЩАЯ СУММА: ${total:F2}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система расчёта стоимости доставки и налогов ===\n");

            //Создаем продукты с указанием веса
            var laptop = new Product("Laptop", 1500m, 2.5);
            var mouse = new Product("Wireless Mouse", 25m, 0.2);
            var keyboard = new Product("Mechanical Keyboard", 75m, 1.2);
            var headphones = new Product("Bluetooth Headphones", 100m, 0.3);
            var usbCable = new Product("USB Cable", 10m, 0.1);
            var book = new Product("Programming Book", 45m, 0.8); // Не электроника

            //Создаем коробки с указанием веса пустой коробки
            var smallBox = new Box("Small Box", 0.2);
            var mediumBox = new Box("Medium Box", 0.5);
            var largeBox = new Box("Large Box", 1.0);

            //Формируем иерархию
            smallBox.Add(mouse);
            smallBox.Add(keyboard);

            mediumBox.Add(smallBox);
            mediumBox.Add(headphones);

            largeBox.Add(mediumBox);
            largeBox.Add(laptop);

            //Создаем заказ
            var order = new Order();
            order.AddComponent(largeBox);
            order.AddComponent(usbCable);
            order.AddComponent(book); // Добавляем не электронный товар

            //Рассчитываем общую стоимость товаров
            decimal subtotal = order.CalculateTotal();
            Console.WriteLine($"Стоимость товаров в заказе: ${subtotal:F2}");

            //Используем посетителя для расчета налогов
            Console.WriteLine("\n--- Расчет налогов ---");
            var taxCalculator = new TaxCalculator();
            order.Accept(taxCalculator);
            Console.WriteLine($"Итого налог: ${taxCalculator.TotalTax:F2}");

            //Используем посетителя для расчета доставки
            Console.WriteLine("\n--- Расчет стоимости доставки ---");
            var deliveryCalculator = new DeliveryCostCalculator();
            order.Accept(deliveryCalculator);
            Console.WriteLine($"Итого стоимость доставки: ${deliveryCalculator.TotalDeliveryCost:F2}");

            //Выводим полную сводку
            order.PrintOrderSummary();

            //Демонстрация повторного использования посетителей
            Console.WriteLine("\n=== Демонстрация повторного использования ===");

            taxCalculator.Reset();
            deliveryCalculator.Reset();

            //Создаем новый простой заказ
            var simpleOrder = new Order();
            simpleOrder.AddComponent(new Product("Smartphone", 800m, 0.3));
            simpleOrder.AddComponent(new Box("Phone Box", 0.3));

            simpleOrder.Accept(taxCalculator);
            simpleOrder.Accept(deliveryCalculator);

            Console.WriteLine($"Налог для простого заказа: ${taxCalculator.TotalTax:F2}");
            Console.WriteLine($"Доставка для простого заказа: ${deliveryCalculator.TotalDeliveryCost:F2}");
        }
    }
}
