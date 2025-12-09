using System;

namespace Chain_of_Responsibility_Pattern
{
    //Класс, представляющий запрос на возврат товара
    public class ReturnRequest
    {
        public string CustomerName { get; set; }
        public int RequestId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public bool IsProcessed { get; set; }
        public string ProcessedBy { get; set; }

        public ReturnRequest(string customerName, int requestId, decimal amount, string reason)
        {
            CustomerName = customerName;
            RequestId = requestId;
            Amount = amount;
            Reason = reason;
            IsProcessed = false;
            ProcessedBy = null;
        }
    }

    //Абстрактный класс обработчика
    public abstract class Handler
    {
        protected Handler successor;

        public void SetSuccessor(Handler successor)
        {
            this.successor = successor;
        }

        public abstract void HandleRequest(ReturnRequest request);
    }

    //Конкретный обработчик - менеджер
    public class ManagerHandler : Handler
    {
        private const decimal MaxAmount = 1000m;

        public override void HandleRequest(ReturnRequest request)
        {
            if (request.Amount <= MaxAmount)
            {
                request.IsProcessed = true;
                request.ProcessedBy = "Менеджер";
                Console.WriteLine($"Запрос #{request.RequestId} обработан менеджером. Сумма: {request.Amount}");
            }
            else if (successor != null)
            {
                Console.WriteLine($"Запрос #{request.RequestId} передан от менеджера руководителю. Сумма: {request.Amount}");
                successor.HandleRequest(request);
            }
            else
            {
                Console.WriteLine($"Запрос #{request.RequestId} не может быть обработан. Достигнут конец цепочки.");
            }
        }
    }

    //Конкретный обработчик - руководитель
    public class SupervisorHandler : Handler
    {
        private const decimal MaxAmount = 5000m;

        public override void HandleRequest(ReturnRequest request)
        {
            if (request.Amount <= MaxAmount)
            {
                request.IsProcessed = true;
                request.ProcessedBy = "Руководитель";
                Console.WriteLine($"Запрос #{request.RequestId} обработан руководителем. Сумма: {request.Amount}");
            }
            else if (successor != null)
            {
                Console.WriteLine($"Запрос #{request.RequestId} передан от руководителя в службу поддержки. Сумма: {request.Amount}");
                successor.HandleRequest(request);
            }
            else
            {
                Console.WriteLine($"Запрос #{request.RequestId} не может быть обработан. Достигнут конец цепочки.");
            }
        }
    }

    //Конкретный обработчик - служба поддержки
    public class SupportHandler : Handler
    {
        public override void HandleRequest(ReturnRequest request)
        {
            //Служба поддержки может обрабатывать любые запросы
            request.IsProcessed = true;
            request.ProcessedBy = "Служба поддержки";
            Console.WriteLine($"Запрос #{request.RequestId} обработан службой поддержки. Сумма: {request.Amount}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система обработки запросов на возврат товара ===\n");

            //Создаем обработчиков
            var manager = new ManagerHandler();
            var supervisor = new SupervisorHandler();
            var support = new SupportHandler();

            //Настраиваем цепочку: менеджер -> руководитель -> служба поддержки
            manager.SetSuccessor(supervisor);
            supervisor.SetSuccessor(support);

            //Создаем тестовые запросы
            var requests = new[]
            {
                new ReturnRequest("Ванёк", 1, 500m, "Бракованный товар"),
                new ReturnRequest("Питер Паркер", 2, 1500m, "Не подошел размер"),
                new ReturnRequest("Железный человек", 3, 6000m, "Ошибка при заказе"),
                new ReturnRequest("ХАХАХАХАЛК", 4, 3000m, "Повреждение при доставке")
            };

            //Обрабатываем каждый запрос через цепочку
            foreach (var request in requests)
            {
                Console.WriteLine($"\nОбработка запроса #{request.RequestId} от {request.CustomerName}:");
                Console.WriteLine($"Сумма: {request.Amount}, Причина: {request.Reason}");
                manager.HandleRequest(request);
                Console.WriteLine($"Статус: {(request.IsProcessed ? $"Обработан ({request.ProcessedBy})" : "Не обработан")}");
            }

            //Демонстрация обработки запроса напрямую через руководителя
            Console.WriteLine("\n=== Прямая обработка через руководителя ===");
            var directRequest = new ReturnRequest("Васильев Дмитрий", 5, 2500m, "Несоответствие описанию");
            supervisor.HandleRequest(directRequest);
        }
    }
}
