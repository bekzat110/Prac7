using System;
using System.Collections.Generic;
using System.Linq;

namespace Pr7
{
    // ===================== COMMAND =======================

    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    // ---------- Устройства умного дома ----------

    public class Light
    {
        public void On() => Console.WriteLine("Свет включен");
        public void Off() => Console.WriteLine("Свет выключен");
    }

    public class Door
    {
        public void Open() => Console.WriteLine("Дверь открыта");
        public void Close() => Console.WriteLine("Дверь закрыта");
    }

    public class Thermostat
    {
        public int Temperature { get; set; } = 20;

        public void Increase() => Console.WriteLine("Температура увеличена");
        public void Decrease() => Console.WriteLine("Температура уменьшена");
    }

    // ----------  Команды для управления устройствами ----------

    public class LightOnCommand : ICommand
    {
        private Light _light;
        public LightOnCommand(Light light) => _light = light;

        public void Execute() => _light.On();
        public void Undo() => _light.Off();
    }

    public class DoorOpenCommand : ICommand
    {
        private Door _door;
        public DoorOpenCommand(Door door) => _door = door;

        public void Execute() => _door.Open();
        public void Undo() => _door.Close();
    }

    public class TemperatureIncreaseCommand : ICommand
    {
        private Thermostat _thermostat;
        public TemperatureIncreaseCommand(Thermostat t) => _thermostat = t;

        public void Execute() => _thermostat.Increase();
        public void Undo() => _thermostat.Decrease();
    }

    public class Invoker
    {
        private Stack<ICommand> _history = new();

        public void ExecuteCommand(ICommand command)
        {
            if (command == null)
            {
                Console.WriteLine("Команда отсутствует");
                return;
            }

            command.Execute();
            _history.Push(command);
        }

        public void Undo()
        {
            if (_history.Count == 0)
            {
                Console.WriteLine("Нет команды для отмены");
                return;
            }

            var cmd = _history.Pop();
            cmd.Undo();
        }
    }

    // ================= TEMPLATE METHOD ====================

    public abstract class Beverage
    {
        public void Prepare()
        {
            BoilWater();
            Brew();
            AddCondiments();
            if (CustomerWantsCondiments())
                Console.WriteLine("Добавки добавлены");
        }

        private void BoilWater() => Console.WriteLine("Кипячение воды");

        protected abstract void Brew();
        protected abstract void AddCondiments();

        protected virtual bool CustomerWantsCondiments()
        {
            while (true)
            {
                Console.WriteLine("Добавить добавки? (y/n)");
                string input = Console.ReadLine()?.ToLower();

                if (input == "y") return true;
                if (input == "n") return false;

                Console.WriteLine("Ошибка ввода!");
            }
        }
    }

    public class Tea : Beverage
    {
        protected override void Brew() => Console.WriteLine("Заваривание чая");
        protected override void AddCondiments() => Console.WriteLine("Добавление лимона");
    }

    public class Coffee : Beverage
    {
        protected override void Brew() => Console.WriteLine("Приготовление кофе");
        protected override void AddCondiments() => Console.WriteLine("Добавление молока");
    }

    public class HotChocolate : Beverage
    {
        protected override void Brew() => Console.WriteLine("Приготовление горячего шоколада");
        protected override void AddCondiments() => Console.WriteLine("Добавление маршмеллоу");
    }

    // ================= MEDIATOR ===========================

    public interface IMediator
    {
        void Register(User user);
        void SendMessage(string message, User sender);
        void SendPrivate(string message, User sender, string receiver);
    }

    public class ChatRoom : IMediator
    {
        private List<User> _users = new();

        public void Register(User user)
        {
            if (!_users.Contains(user))
            {
                _users.Add(user);
                Console.WriteLine($"{user.Name} присоединился к чату");
            }
        }

        public void SendMessage(string message, User sender)
        {
            if (!_users.Contains(sender))
            {
                Console.WriteLine("Пользователь не в чате");
                return;
            }

            foreach (var user in _users)
            {
                if (user != sender)
                    user.Receive($"{sender.Name}: {message}");
            }
        }

        public void SendPrivate(string message, User sender, string receiver)
        {
            var user = _users.FirstOrDefault(x => x.Name == receiver);

            if (user == null)
            {
                Console.WriteLine("Получатель не найден");
                return;
            }

            user.Receive($"[Private] {sender.Name}: {message}");
        }
    }

    public class User
    {
        public string Name { get; }
        private IMediator _mediator;

        public User(string name, IMediator mediator)
        {
            Name = name;
            _mediator = mediator;
        }

        public void Send(string message)
        {
            _mediator.SendMessage(message, this);
        }

        public void SendPrivate(string message, string receiver)
        {
            _mediator.SendPrivate(message, this, receiver);
        }

        public void Receive(string message)
        {
            Console.WriteLine($"[{Name}] {message}");
        }
    }

    // ================= MAIN ===============================

    class Program
    {
        static void Main()
        {
            Console.WriteLine("===== COMMAND =====");

            var invoker = new Invoker();

            invoker.ExecuteCommand(new LightOnCommand(new Light()));
            invoker.Undo();

            Console.WriteLine("\n===== TEMPLATE METHOD =====");

            Beverage tea = new Tea();
            tea.Prepare();

            Beverage coffee = new Coffee();
            coffee.Prepare();

            Console.WriteLine("\n===== MEDIATOR =====");

            IMediator chat = new ChatRoom();

            var user1 = new User("Bekzat", chat);
            var user2 = new User("Ali", chat);

            chat.Register(user1);
            chat.Register(user2);

            user1.Send("Hello!");
            user1.SendPrivate("Private message", "Ali");
        }
    }
}