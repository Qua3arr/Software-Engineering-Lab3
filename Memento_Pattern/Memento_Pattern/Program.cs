using System;
using System.Collections.Generic;
using System.Linq;

namespace Memento_Pattern
{
    //Класс Memento - хранит состояние корзины
    public class ShoppingCartMemento
    {
        public List<string> Items { get; }
        public DateTime SnapshotDate { get; }

        public ShoppingCartMemento(List<string> items)
        {
            Items = new List<string>(items);
            SnapshotDate = DateTime.Now;
        }

        public void Display()
        {
            Console.WriteLine($"Снимок от {SnapshotDate:HH:mm:ss}");
            Console.WriteLine($"Товары: {string.Join(", ", Items)}");
            Console.WriteLine();
        }
    }

    //Класс Caretaker - управляет мементами
    public class ShoppingCartCaretaker
    {
        private readonly Stack<ShoppingCartMemento> _history = new Stack<ShoppingCartMemento>();
        private readonly ShoppingCart _cart;

        public ShoppingCartCaretaker(ShoppingCart cart)
        {
            _cart = cart;
        }

        //Сохранить текущее состояние
        public void Save()
        {
            var memento = _cart.CreateMemento();
            _history.Push(memento);
            Console.WriteLine($"Состояние сохранено. Всего сохранений: {_history.Count}");
            memento.Display();
        }

        //Отменить последнее изменение
        public bool Undo()
        {
            if (_history.Count == 0)
            {
                Console.WriteLine("Нет сохраненных состояний для отмены.");
                return false;
            }

            var memento = _history.Pop();
            _cart.Restore(memento);
            Console.WriteLine($"Восстановлено состояние от {memento.SnapshotDate:HH:mm:ss}");
            Console.WriteLine($"Осталось сохранений: {_history.Count}");
            return true;
        }

        //Показать историю
        public void ShowHistory()
        {
            if (_history.Count == 0)
            {
                Console.WriteLine("История пуста.");
                return;
            }

            Console.WriteLine($"История изменений ({_history.Count} сохранений):");
            var i = 1;
            foreach (var memento in _history.Reverse())
            {
                Console.Write($"{i++}. ");
                memento.Display();
            }
        }
    }

    //Класс корзины покупок
    public class ShoppingCart
    {
        private List<string> _items = new List<string>();

        public IReadOnlyList<string> Items => _items.AsReadOnly();
        public int ItemCount => _items.Count;
        public decimal TotalPrice => _items.Count * 10;

        //Добавить товар
        public void AddItem(string item)
        {
            _items.Add(item);
            Console.WriteLine($"Добавлен товар: {item}");
            DisplayCart();
        }

        //Удалить товар
        public bool RemoveItem(string item)
        {
            if (_items.Remove(item))
            {
                Console.WriteLine($"Удален товар: {item}");
                DisplayCart();
                return true;
            }
            Console.WriteLine($"Товар '{item}' не найден в корзине.");
            return false;
        }

        //Очистить корзину
        public void Clear()
        {
            _items.Clear();
            Console.WriteLine("Корзина очищена.");
            DisplayCart();
        }

        //Создать снимок состояния
        public ShoppingCartMemento CreateMemento()
        {
            return new ShoppingCartMemento(_items);
        }

        //Восстановить состояние из снимка
        public void Restore(ShoppingCartMemento memento)
        {
            _items = new List<string>(memento.Items);
            Console.WriteLine("Состояние корзины восстановлено.");
            DisplayCart();
        }

        //Отобразить текущее состояние корзины
        public void DisplayCart()
        {
            Console.WriteLine("=== ТЕКУЩАЯ КОРЗИНА ===");
            if (_items.Count == 0)
            {
                Console.WriteLine("Корзина пуста");
            }
            else
            {
                Console.WriteLine($"Товаров в корзине: {ItemCount}");
                Console.WriteLine($"Общая стоимость: ${TotalPrice}");
                Console.WriteLine("Список товаров:");
                for (int i = 0; i < _items.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {_items[i]}");
                }
            }
            Console.WriteLine("=====================\n");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== СИСТЕМА УПРАВЛЕНИЯ КОРЗИНОЙ ПОКУПОК ===\n");

            //Создаём корзину
            var cart = new ShoppingCart();
            var caretaker = new ShoppingCartCaretaker(cart);

            //Пользовательский интерфейс
            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Добавить товар");
                Console.WriteLine("2. Удалить товар");
                Console.WriteLine("3. Очистить корзину");
                Console.WriteLine("4. Сохранить состояние");
                Console.WriteLine("5. Отменить последнее действие (Undo)");
                Console.WriteLine("6. Показать историю");
                Console.WriteLine("7. Показать текущую корзину");
                Console.WriteLine("8. Выход");
                Console.Write("Ваш выбор: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Неверный ввод!\n");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Введите название товара: ");
                        var itemToAdd = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(itemToAdd))
                        {
                            cart.AddItem(itemToAdd);
                        }
                        break;

                    case 2:
                        Console.Write("Введите название товара для удаления: ");
                        var itemToRemove = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(itemToRemove))
                        {
                            cart.RemoveItem(itemToRemove);
                        }
                        break;

                    case 3:
                        cart.Clear();
                        break;

                    case 4:
                        caretaker.Save();
                        break;

                    case 5:
                        caretaker.Undo();
                        break;

                    case 6:
                        caretaker.ShowHistory();
                        break;

                    case 7:
                        cart.DisplayCart();
                        break;

                    case 8:
                        Console.WriteLine("Выход из программы...");
                        return;

                    default:
                        Console.WriteLine("Неверный выбор!\n");
                        break;
                }

                Console.WriteLine();
            }
        }
    }
}
