using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command_Pattern
{

    //Класс помещения
    public class Room
    {
        public string Name { get; set; }

        public Room(string name)
        {
            Name = name;
        }
    }

    //Класс этажа
    public class Floor
    {
        public int Number { get; set; }
        public List<Room> Rooms { get; set; }

        public Floor(int number, int roomsCount)
        {
            Number = number;
            Rooms = new List<Room>();

            for (int i = 1; i <= roomsCount; i++)
            {
                Rooms.Add(new Room($"Комната {number}-{i}"));
            }
        }
    }

    //Singleton класс Здания
    public class Building
    {
        private static Building instance;

        public List<Floor> Floors { get; set; }
        public string Name { get; set; }

        private Building(int floorsCount, int roomsPerFloor)
        {
            Name = "Многоэтажное здание";
            Floors = new List<Floor>();

            for (int i = 1; i <= floorsCount; i++)
            {
                Floors.Add(new Floor(i, roomsPerFloor));
            }

            Console.WriteLine("Создано здание с {0} этажами", floorsCount);
        }

        public static Building GetInstance(int floorsCount, int roomsPerFloor)
        {
            if (instance == null)
                instance = new Building(floorsCount, roomsPerFloor);
            return instance;
        }

        public static Building GetExistingInstance()
        {
            if (instance == null)
                throw new Exception("Здание еще не создано. Сначала вызовите GetInstance.");
            return instance;
        }

        public void DisplayInfo()
        {
            Console.WriteLine("\n=== {0} ===", Name);
            Console.WriteLine("Этажей: {0}", Floors.Count);
            foreach (var floor in Floors)
            {
                Console.WriteLine("Этаж {0}: {1} помещений", floor.Number, floor.Rooms.Count);
            }
        }
    }

    //Singleton класс Лифта с добавлением состояния дверей
    public class Elevator
    {
        private static Elevator instance;
        private Building _building;

        public int CurrentFloor { get; private set; }
        public bool IsMoving { get; private set; }
        public bool IsDoorOpen { get; private set; }

        private Elevator()
        {
            CurrentFloor = 1;
            IsMoving = false;
            IsDoorOpen = false;

            _building = Building.GetExistingInstance();

            Console.WriteLine("\nЛифт создан и находится на 1 этаже");
            Console.WriteLine("Двери закрыты");
        }

        public static Elevator GetInstance()
        {
            if (instance == null)
                instance = new Elevator();
            return instance;
        }

        public void MoveToFloor(int targetFloor)
        {
            if (IsMoving)
            {
                Console.WriteLine("Лифт уже движется!");
                return;
            }

            if (IsDoorOpen)
            {
                Console.WriteLine("Невозможно двигаться с открытыми дверями!");
                return;
            }

            if (targetFloor < 1 || targetFloor > _building.Floors.Count)
            {
                Console.WriteLine($"Неверный этаж! Допустимые этажи: 1-{_building.Floors.Count}");
                return;
            }

            if (targetFloor == CurrentFloor)
            {
                Console.WriteLine("Лифт уже на этом этаже!");
                return;
            }

            IsMoving = true;
            string direction = targetFloor > CurrentFloor ? "вверх" : "вниз";
            Console.WriteLine($"Лифт едет {direction} с {CurrentFloor} этажа на {targetFloor} этаж...");

            System.Threading.Thread.Sleep(1000);

            CurrentFloor = targetFloor;
            IsMoving = false;
            Console.WriteLine($"Лифт прибыл на {CurrentFloor} этаж");
        }

        public bool MoveUp()
        {
            int targetFloor = CurrentFloor + 1;
            if (targetFloor <= _building.Floors.Count)
            {
                MoveToFloor(targetFloor);
                return true;
            }
            else
            {
                Console.WriteLine("Невозможно подняться выше!");
                return false;
            }
        }

        public bool MoveDown()
        {
            int targetFloor = CurrentFloor - 1;
            if (targetFloor >= 1)
            {
                MoveToFloor(targetFloor);
                return true;
            }
            else
            {
                Console.WriteLine("Невозможно опуститься ниже!");
                return false;
            }
        }

        public void OpenDoor()
        {
            if (IsMoving)
            {
                Console.WriteLine("Невозможно открыть двери во время движения!");
                return;
            }

            if (IsDoorOpen)
            {
                Console.WriteLine("Двери уже открыты!");
                return;
            }

            IsDoorOpen = true;
            Console.WriteLine("Двери лифта открыты");
        }

        public void CloseDoor()
        {
            if (!IsDoorOpen)
            {
                Console.WriteLine("Двери уже закрыты!");
                return;
            }

            IsDoorOpen = false;
            Console.WriteLine("Двери лифта закрыты");
        }

        public void DisplayStatus()
        {
            Console.WriteLine("\n=== Статус лифта ===");
            Console.WriteLine($"Текущий этаж: {CurrentFloor}/{_building.Floors.Count}");
            Console.WriteLine($"Двери: {(IsDoorOpen ? "Открыты" : "Закрыты")}");
            Console.WriteLine($"Движение: {(IsMoving ? "В процессе" : "Остановлен")}");
            Console.WriteLine("===================\n");
        }
    }

    //Паттерн Command
    //Абстрактный класс команды
    public abstract class Command
    {
        public abstract void Execute();
        public abstract void Undo();
    }

    //Конкретные команды
    public class MoveUpCommand : Command
    {
        private Elevator _elevator;
        private bool _wasExecutedSuccessfully;

        public MoveUpCommand(Elevator elevator)
        {
            _elevator = elevator;
            _wasExecutedSuccessfully = false;
        }

        public override void Execute()
        {
            _wasExecutedSuccessfully = _elevator.MoveUp();
        }

        public override void Undo()
        {
            if (_wasExecutedSuccessfully)
            {
                Console.WriteLine("Отмена движения вверх...");
                _elevator.MoveDown();
            }
        }
    }

    public class MoveDownCommand : Command
    {
        private Elevator _elevator;
        private bool _wasExecutedSuccessfully;

        public MoveDownCommand(Elevator elevator)
        {
            _elevator = elevator;
            _wasExecutedSuccessfully = false;
        }

        public override void Execute()
        {
            _wasExecutedSuccessfully = _elevator.MoveDown();
        }

        public override void Undo()
        {
            if (_wasExecutedSuccessfully)
            {
                Console.WriteLine("Отмена движения вниз...");
                _elevator.MoveUp();
            }
        }
    }

    public class OpenDoorCommand : Command
    {
        private Elevator _elevator;
        private bool _wasDoorOpened;

        public OpenDoorCommand(Elevator elevator)
        {
            _elevator = elevator;
            _wasDoorOpened = false;
        }

        public override void Execute()
        {
            try
            {
                _elevator.OpenDoor();
                _wasDoorOpened = true;
            }
            catch
            {
                _wasDoorOpened = false;
            }
        }

        public override void Undo()
        {
            if (_wasDoorOpened)
            {
                Console.WriteLine("Отмена открытия дверей...");
                _elevator.CloseDoor();
            }
        }
    }

    public class CloseDoorCommand : Command
    {
        private Elevator _elevator;
        private bool _wasDoorClosed;

        public CloseDoorCommand(Elevator elevator)
        {
            _elevator = elevator;
            _wasDoorClosed = false;
        }

        public override void Execute()
        {
            try
            {
                _elevator.CloseDoor();
                _wasDoorClosed = true;
            }
            catch
            {
                _wasDoorClosed = false;
            }
        }

        public override void Undo()
        {
            if (_wasDoorClosed)
            {
                Console.WriteLine("Отмена закрытия дверей...");
                _elevator.OpenDoor();
            }
        }
    }

    //Команда для перемещения на конкретный этаж
    public class MoveToFloorCommand : Command
    {
        private Elevator _elevator;
        private int _targetFloor;
        private int _previousFloor;
        private bool _wasExecutedSuccessfully;

        public MoveToFloorCommand(Elevator elevator, int targetFloor)
        {
            _elevator = elevator;
            _targetFloor = targetFloor;
            _wasExecutedSuccessfully = false;
        }

        public override void Execute()
        {
            _previousFloor = _elevator.CurrentFloor;
            try
            {
                _elevator.MoveToFloor(_targetFloor);
                _wasExecutedSuccessfully = true;
            }
            catch
            {
                _wasExecutedSuccessfully = false;
            }
        }

        public override void Undo()
        {
            if (_wasExecutedSuccessfully && _previousFloor != _targetFloor)
            {
                Console.WriteLine($"Отмена перемещения на этаж {_targetFloor}...");
                _elevator.MoveToFloor(_previousFloor);
            }
        }
    }

    //История команд
    public class CommandHistory
    {
        private Stack<Command> _history = new Stack<Command>();
        private Stack<Command> _redoStack = new Stack<Command>();

        public void Push(Command command)
        {
            _history.Push(command);
            _redoStack.Clear();
        }

        public bool CanUndo()
        {
            return _history.Count > 0;
        }

        public bool CanRedo()
        {
            return _redoStack.Count > 0;
        }

        public void Undo()
        {
            if (_history.Count > 0)
            {
                Command command = _history.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
            else
            {
                Console.WriteLine("Нет команд для отмены!");
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                Command command = _redoStack.Pop();
                command.Execute();
                _history.Push(command);
            }
            else
            {
                Console.WriteLine("Нет команд для повторного выполнения!");
            }
        }

        public void DisplayHistory()
        {
            Console.WriteLine("\n=== История команд ===");

            if (_history.Count == 0)
            {
                Console.WriteLine("История пуста");
            }
            else
            {
                var tempArray = _history.ToArray();

                for (int i = tempArray.Length - 1; i >= 0; i--)
                {
                    string commandName = tempArray[i].GetType().Name;
                    int commandNumber = tempArray.Length - i;
                    Console.WriteLine($"{commandNumber}. {commandName}");
                }
            }

            Console.WriteLine("=====================\n");
        }
    }

    //Контроллер лифта
    public class LiftControl
    {
        private CommandHistory _history = new CommandHistory();
        private Elevator _elevator;

        public LiftControl(Elevator elevator)
        {
            _elevator = elevator;
        }

        public void ExecuteCommand(Command command)
        {
            command.Execute();
            _history.Push(command);
        }

        public void UndoLastCommand()
        {
            _history.Undo();
        }

        public void RedoLastUndo()
        {
            _history.Redo();
        }

        public void DisplayHistory()
        {
            _history.DisplayHistory();
        }

        public void DisplayStatus()
        {
            _elevator.DisplayStatus();
        }

        public void DirectMoveToFloor(int floor)
        {
            ExecuteCommand(new MoveToFloorCommand(_elevator, floor));
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система управления лифтом с паттерном Command ===\n");

            //Создаем здание (Singleton)
            Building building = Building.GetInstance(5, 3);
            building.DisplayInfo();

            //Получаем экземпляр лифта (Singleton)
            Elevator elevator = Elevator.GetInstance();

            //Создаем контроллер лифта
            LiftControl control = new LiftControl(elevator);

            //Демонстрация работы
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n===== Меню управления лифтом =====");
                Console.WriteLine("1. Подняться на один этаж вверх");
                Console.WriteLine("2. Опуститься на один этаж вниз");
                Console.WriteLine("3. Открыть двери");
                Console.WriteLine("4. Закрыть двери");
                Console.WriteLine("5. Переместиться на конкретный этаж");
                Console.WriteLine("6. Отменить последнее действие");
                Console.WriteLine("7. Повторить отмененное действие");
                Console.WriteLine("8. Показать историю команд");
                Console.WriteLine("9. Показать статус лифта");
                Console.WriteLine("10. Показать информацию о здании");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        control.ExecuteCommand(new MoveUpCommand(elevator));
                        break;

                    case "2":
                        control.ExecuteCommand(new MoveDownCommand(elevator));
                        break;

                    case "3":
                        control.ExecuteCommand(new OpenDoorCommand(elevator));
                        break;

                    case "4":
                        control.ExecuteCommand(new CloseDoorCommand(elevator));
                        break;

                    case "5":
                        Console.Write("Введите номер этажа (1-5): ");
                        if (int.TryParse(Console.ReadLine(), out int floor))
                        {
                            control.DirectMoveToFloor(floor);
                        }
                        else
                        {
                            Console.WriteLine("Неверный формат этажа!");
                        }
                        break;

                    case "6":
                        control.UndoLastCommand();
                        break;

                    case "7":
                        control.RedoLastUndo();
                        break;

                    case "8":
                        control.DisplayHistory();
                        break;

                    case "9":
                        control.DisplayStatus();
                        break;

                    case "10":
                        building.DisplayInfo();
                        break;

                    case "0":
                        running = false;
                        Console.WriteLine("Выход из системы управления лифтом.");
                        break;

                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }

            //Демонстрация работы с историей команд
            Console.WriteLine("\n=== Демонстрация работы паттерна Command ===");

            //Выполняем последовательность команд
            Console.WriteLine("\n1. Выполняем последовательность команд:");
            control.ExecuteCommand(new OpenDoorCommand(elevator));
            control.ExecuteCommand(new CloseDoorCommand(elevator));
            control.ExecuteCommand(new MoveUpCommand(elevator));
            control.ExecuteCommand(new MoveUpCommand(elevator));

            control.DisplayStatus();
            control.DisplayHistory();

            //Отменяем две последние команды
            Console.WriteLine("\n2. Отменяем две последние команды:");
            control.UndoLastCommand();
            control.UndoLastCommand();

            control.DisplayStatus();
            control.DisplayHistory();

            //Повторяем отмененное действие
            Console.WriteLine("\n3. Повторяем отмененное действие:");
            control.RedoLastUndo();

            control.DisplayStatus();
            control.DisplayHistory();

            //Пытаемся создать еще одно здание (не получится - Singleton)
            Console.WriteLine("\n4. Пытаемся создать еще одно здание:");
            Building anotherBuilding = Building.GetInstance(10, 5);
            anotherBuilding.DisplayInfo();
        }
    }
}
