using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelAndStockSystem
{
    // STRATEGY PATTERN: Travel Booking

    public interface ICostCalculationStrategy
    {
        double CalculateCost(double distance, int passengers, string serviceClass, double basePrice, double discount);
    }

    // Конкретные стратегии
    public class PlaneStrategy : ICostCalculationStrategy
    {
        public double CalculateCost(double distance, int passengers, string serviceClass, double basePrice, double discount)
        {
            double multiplier = serviceClass.ToLower() == "бизнес" ? 2.0 : 1.0;
            return (basePrice * distance * multiplier * passengers) * (1 - discount);
        }
    }

    public class TrainStrategy : ICostCalculationStrategy
    {
        public double CalculateCost(double distance, int passengers, string serviceClass, double basePrice, double discount)
        {
            double multiplier = serviceClass.ToLower() == "бизнес" ? 1.5 : 1.0;
            return (basePrice * distance * multiplier * passengers) * (1 - discount);
        }
    }

    public class BusStrategy : ICostCalculationStrategy
    {
        public double CalculateCost(double distance, int passengers, string serviceClass, double basePrice, double discount)
        {
            double multiplier = 1.0; // автобусы эконом
            return (basePrice * distance * multiplier * passengers) * (1 - discount);
        }
    }

    // Контекст
    public class TravelBookingContext
    {
        private ICostCalculationStrategy _strategy;

        public void SetStrategy(ICostCalculationStrategy strategy)
        {
            _strategy = strategy;
        }

        public double GetCost(double distance, int passengers, string serviceClass, double basePrice, double discount)
        {
            if (_strategy == null) throw new InvalidOperationException("Стратегия расчета не выбрана!");
            return _strategy.CalculateCost(distance, passengers, serviceClass, basePrice, discount);
        }
    }

    // OBSERVER PATTERN: Stock Exchange

    public interface IObserver
    {
        void Update(string stock, double price);
    }

    public interface ISubject
    {
        void AddObserver(string stock, IObserver observer);
        void RemoveObserver(string stock, IObserver observer);
        void NotifyObservers(string stock, double price);
    }

    // Субъект
    public class StockExchange : ISubject
    {
        private Dictionary<string, List<IObserver>> _subscriptions = new Dictionary<string, List<IObserver>>();

        public void AddObserver(string stock, IObserver observer)
        {
            if (!_subscriptions.ContainsKey(stock))
                _subscriptions[stock] = new List<IObserver>();

            if (!_subscriptions[stock].Contains(observer))
                _subscriptions[stock].Add(observer);

            Console.WriteLine($"Наблюдатель добавлен на {stock}");
        }

        public void RemoveObserver(string stock, IObserver observer)
        {
            if (_subscriptions.ContainsKey(stock))
            {
                _subscriptions[stock].Remove(observer);
                Console.WriteLine($"Наблюдатель удален с {stock}");
            }
        }

        public void NotifyObservers(string stock, double price)
        {
            if (_subscriptions.ContainsKey(stock))
            {
                foreach (var observer in _subscriptions[stock])
                {
                    observer.Update(stock, price);
                }
            }
        }

        // Имитация изменения цен
        public async Task SimulateMarketAsync()
        {
            Random rnd = new Random();
            string[] stocks = _subscriptions.Keys.ToArray();
            for (int i = 0; i < 5; i++)
            {
                foreach (var stock in stocks)
                {
                    double newPrice = rnd.Next(50, 200);
                    Console.WriteLine($"\n[Биржа] {stock} изменил цену на {newPrice}");
                    NotifyObservers(stock, newPrice);
                    await Task.Delay(500);
                }
            }
        }
    }

    // Конкретные наблюдатели
    public class Trader : IObserver
    {
        private double _threshold;
        public Trader(double threshold) { _threshold = threshold; }

        public void Update(string stock, double price)
        {
            Console.WriteLine($"Трейдер уведомлен: {stock} = {price}");
            if (price > _threshold) Console.WriteLine($"Трейдер: {stock} выше {_threshold}, продаю!");
        }
    }

    public class AutoRobot : IObserver
    {
        private double _buyThreshold;
        private double _sellThreshold;
        public AutoRobot(double buyThreshold, double sellThreshold)
        {
            _buyThreshold = buyThreshold;
            _sellThreshold = sellThreshold;
        }

        public void Update(string stock, double price)
        {
            if (price < _buyThreshold)
                Console.WriteLine($"Робот: {stock} ниже {_buyThreshold}, покупаю!");
            else if (price > _sellThreshold)
                Console.WriteLine($"Робот: {stock} выше {_sellThreshold}, продаю!");
            else
                Console.WriteLine($"Робот: {stock} цена {price}, держу позиции.");
        }
    }

    public class MobileApp : IObserver
    {
        public void Update(string stock, double price)
        {
            Console.WriteLine($"Мобильное приложение: {stock} = {price}");
        }
    }

    // MAIN

    class Program
    {
        static async Task Main(string[] args)
        {
            // ===== TRAVEL BOOKING =====
            Console.WriteLine("===== Travel Booking System =====\n");
            TravelBookingContext travelContext = new TravelBookingContext();

            Console.WriteLine("Выберите транспорт: 1-Самолет, 2-Поезд, 3-Автобус");
            int transport = int.Parse(Console.ReadLine() ?? "1");

            switch (transport)
            {
                case 1: travelContext.SetStrategy(new PlaneStrategy()); break;
                case 2: travelContext.SetStrategy(new TrainStrategy()); break;
                case 3: travelContext.SetStrategy(new BusStrategy()); break;
                default: travelContext.SetStrategy(new BusStrategy()); break;
            }

            Console.Write("Введите расстояние (км): ");
            double distance = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("Введите количество пассажиров: ");
            int passengers = int.Parse(Console.ReadLine() ?? "1");

            Console.Write("Введите класс обслуживания (Эконом/Бизнес): ");
            string serviceClass = Console.ReadLine() ?? "Эконом";

            Console.Write("Введите скидку (например, 0.1 для 10%): ");
            double discount = double.Parse(Console.ReadLine() ?? "0");

            double basePrice = 10; // базовая цена за км
            double totalCost = travelContext.GetCost(distance, passengers, serviceClass, basePrice, discount);

            Console.WriteLine($"\nОбщая стоимость поездки: {totalCost}\n");

            // ===== STOCK EXCHANGE =====
            Console.WriteLine("===== Stock Exchange System =====\n");

            StockExchange exchange = new StockExchange();

            Trader trader1 = new Trader(150);
            AutoRobot robot1 = new AutoRobot(70, 180);
            MobileApp app = new MobileApp();

            // Подписка на акции
            exchange.AddObserver("AAPL", trader1);
            exchange.AddObserver("AAPL", robot1);
            exchange.AddObserver("GOOG", robot1);
            exchange.AddObserver("AAPL", app);
            exchange.AddObserver("GOOG", app);

            // Симуляция биржи
            await exchange.SimulateMarketAsync();

            Console.WriteLine("\nПрограмма завершена.");
        }
    }
}