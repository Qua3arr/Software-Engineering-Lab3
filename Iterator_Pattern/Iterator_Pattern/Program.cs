using System;
using System.Collections.Generic;
using System.Linq;

namespace Iterator_Pattern
{
    //Интерфейс итератора
    public interface ICatalogIterator
    {
        bool HasNext();
        Product Next();
        List<Product> Next(int count);
        void Reset();
    }

    //Класс товара
    public class Product
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Popularity { get; set; }

        public Product(string name, string category, decimal price, int popularity)
        {
            Name = name;
            Category = category;
            Price = price;
            Popularity = popularity;
        }

        public override string ToString()
        {
            return $"{Name} (Категория: {Category}, Цена: {Price}, Популярность: {Popularity}/10)";
        }
    }

    //Итератор по категориям (группировка по категориям)
    public class CategoryIterator : ICatalogIterator
    {
        private List<Product> _products;
        private List<string> _categories;
        private int _currentCategoryIndex;
        private int _currentProductIndex;

        public CategoryIterator(List<Product> products)
        {
            _products = products.OrderBy(p => p.Category).ThenBy(p => p.Name).ToList();
            _categories = _products.Select(p => p.Category).Distinct().ToList();
            Reset();
        }

        public bool HasNext()
        {
            if (_currentCategoryIndex >= _categories.Count) return false;

            var currentCategory = _categories[_currentCategoryIndex];
            var categoryProducts = _products.Where(p => p.Category == currentCategory).ToList();

            return _currentProductIndex < categoryProducts.Count;
        }

        public Product Next()
        {
            if (!HasNext()) return null;

            var currentCategory = _categories[_currentCategoryIndex];
            var categoryProducts = _products.Where(p => p.Category == currentCategory).ToList();
            var product = categoryProducts[_currentProductIndex];

            _currentProductIndex++;

            if (_currentProductIndex >= categoryProducts.Count)
            {
                _currentCategoryIndex++;
                _currentProductIndex = 0;
            }

            return product;
        }

        public List<Product> Next(int count)
        {
            var result = new List<Product>();
            for (int i = 0; i < count && HasNext(); i++)
            {
                result.Add(Next());
            }
            return result;
        }

        public void Reset()
        {
            _currentCategoryIndex = 0;
            _currentProductIndex = 0;
        }
    }

    //Итератор по цене (от дешевых к дорогим)
    public class PriceIterator : ICatalogIterator
    {
        private List<Product> _products;
        private int _currentIndex;

        public PriceIterator(List<Product> products)
        {
            _products = products.OrderBy(p => p.Price).ThenBy(p => p.Name).ToList();
            Reset();
        }

        public bool HasNext()
        {
            return _currentIndex < _products.Count;
        }

        public Product Next()
        {
            if (!HasNext()) return null;
            return _products[_currentIndex++];
        }

        public List<Product> Next(int count)
        {
            var result = new List<Product>();
            for (int i = 0; i < count && HasNext(); i++)
            {
                result.Add(Next());
            }
            return result;
        }

        public void Reset()
        {
            _currentIndex = 0;
        }
    }

    //Итератор по популярности (от самых популярных)
    public class PopularityIterator : ICatalogIterator
    {
        private List<Product> _products;
        private int _currentIndex;

        public PopularityIterator(List<Product> products)
        {
            _products = products.OrderByDescending(p => p.Popularity).ThenBy(p => p.Name).ToList();
            Reset();
        }

        public bool HasNext()
        {
            return _currentIndex < _products.Count;
        }

        public Product Next()
        {
            if (!HasNext()) return null;
            return _products[_currentIndex++];
        }

        public List<Product> Next(int count)
        {
            var result = new List<Product>();
            for (int i = 0; i < count && HasNext(); i++)
            {
                result.Add(Next());
            }
            return result;
        }

        public void Reset()
        {
            _currentIndex = 0;
        }
    }

    //Класс каталога
    public class Catalog
    {
        private List<Product> _products;
        private ICatalogIterator _currentIterator;

        public Catalog()
        {
            _products = new List<Product>();
            SetDefaultProducts();
        }

        private void SetDefaultProducts()
        {
            _products.Add(new Product("Ноутбук Asus", "Электроника", 75000, 8));
            _products.Add(new Product("Смартфон Samsung", "Электроника", 45000, 9));
            _products.Add(new Product("Футболка", "Одежда", 1500, 6));
            _products.Add(new Product("Спортивные штаны", "Одежда", 3500, 7));
            _products.Add(new Product("Книга 'Метро 2033'", "Книги", 800, 10));
            _products.Add(new Product("Наушники Sony", "Электроника", 12000, 8));
            _products.Add(new Product("Кофеварка", "Бытовая техника", 5000, 7));
            _products.Add(new Product("Роман 'Мастер и Маргарита'", "Книги", 600, 9));
            _products.Add(new Product("Кроссовки Nike", "Одежда", 7000, 8));
            _products.Add(new Product("Холодильник LG", "Бытовая техника", 40000, 6));
        }

        //Установка стратегии обхода
        public void SetIteratorStrategy(string strategy)
        {
            switch (strategy.ToLower())
            {
                case "category":
                    _currentIterator = new CategoryIterator(_products);
                    break;
                case "price":
                    _currentIterator = new PriceIterator(_products);
                    break;
                case "popularity":
                    _currentIterator = new PopularityIterator(_products);
                    break;
                default:
                    throw new ArgumentException($"Неизвестная стратегия: {strategy}");
            }
        }

        //Получение текущего итератора
        public ICatalogIterator GetIterator()
        {
            if (_currentIterator == null)
            {
                _currentIterator = new CategoryIterator(_products);
            }
            return _currentIterator;
        }

        //Добавление товара
        public void AddProduct(Product product)
        {
            _products.Add(product);
            if (_currentIterator != null)
            {
                SetIteratorStrategy(GetCurrentStrategyName());
            }
        }

        private string GetCurrentStrategyName()
        {
            if (_currentIterator is CategoryIterator) return "category";
            if (_currentIterator is PriceIterator) return "price";
            if (_currentIterator is PopularityIterator) return "popularity";
            return "category";
        }

        //Показать все товары с использованием текущего итератора
        public void DisplayProducts()
        {
            var iterator = GetIterator();
            iterator.Reset();

            Console.WriteLine("\n=== Обход каталога ===");
            int count = 1;
            while (iterator.HasNext())
            {
                var product = iterator.Next();
                Console.WriteLine($"{count++}. {product}");
            }
            Console.WriteLine("=====================\n");
        }

        //Показать несколько товаров
        public void DisplayProducts(int count)
        {
            var iterator = GetIterator();
            iterator.Reset();

            Console.WriteLine($"\n=== Первые {count} товаров ===");
            var products = iterator.Next(count);
            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {products[i]}");
            }
            Console.WriteLine("=====================\n");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var catalog = new Catalog();

            Console.WriteLine("Демонстрация паттерна Iterator для каталога интернет-магазина\n");

            //Обход по категориям
            catalog.SetIteratorStrategy("category");
            Console.WriteLine("1. Обход по категориям:");
            catalog.DisplayProducts();

            //Обход по цене (от дешевых к дорогим)
            catalog.SetIteratorStrategy("price");
            Console.WriteLine("2. Обход по цене (от дешевых к дорогим):");
            catalog.DisplayProducts();

            //Обход по популярности
            catalog.SetIteratorStrategy("popularity");
            Console.WriteLine("3. Обход по популярности (самые популярные сначала):");
            catalog.DisplayProducts();

            //Демонстрация метода Next(int count)
            Console.WriteLine("4. Показать первые 3 товара по популярности:");
            catalog.DisplayProducts(3);

            //Демонстрация добавления нового товара
            Console.WriteLine("5. Добавление нового товара и обход по цене:");
            catalog.AddProduct(new Product("Манипулятор энергетического поля нулевого уровня", "Электроника", 3000000, 10));
            catalog.SetIteratorStrategy("price");
            catalog.DisplayProducts();

            //Демонстрация ручного использования итератора
            Console.WriteLine("6. Ручное использование итератора по категориям:");
            catalog.SetIteratorStrategy("category");
            var iterator = catalog.GetIterator();

            Console.WriteLine("Первые два товара:");
            var firstTwo = iterator.Next(2);
            foreach (var product in firstTwo)
            {
                Console.WriteLine($"  - {product}");
            }

            Console.WriteLine("\nСледующие три товара:");
            var nextThree = iterator.Next(3);
            foreach (var product in nextThree)
            {
                Console.WriteLine($"  - {product}");
            }

            Console.WriteLine("\nСброс итератора и вывод первых двух товаров:");
            iterator.Reset();
            firstTwo = iterator.Next(2);
            foreach (var product in firstTwo)
            {
                Console.WriteLine($"  - {product}");
            }
        }
    }
}
