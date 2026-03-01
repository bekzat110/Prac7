using System;
using System.Collections.Generic;

namespace DesignPatternsModule06
{
    // STRATEGY PATTERN (Оплата)

    // 1. Интерфейс стратегии
    public interface IPaymentStrategy
    {
        void Pay(double amount);
    }

    // 2. Реализация стратегий

    public class CreditCardPayment : IPaymentStrategy
    {
        private string _cardNumber;

        public CreditCardPayment(string cardNumber)
        {
            _cardNumber = cardNumber;
        }

        public void Pay(double amount)
        {
            Console.WriteLine($"Оплата {amount} тг банковской картой: {_cardNumber}");
        }
    }

    public class PayPalPayment : IPaymentStrategy
    {
        private string _email;

        public PayPalPayment(string email)
        {
            _email = email;
        }

        public void Pay(double amount)
        {
            Console.WriteLine($"Оплата {amount} тг через PayPal: {_email}");
        }
    }

    public class CryptoPayment : IPaymentStrategy
    {
        private string _wallet;

        public CryptoPayment(string wallet)
        {
            _wallet = wallet;
        }

        public void Pay(double amount)
        {
            Console.WriteLine($"Оплата {amount} тг криптовалютой. Кошелек: {_wallet}");
        }
    }

    // 3. Контекст
    public class PaymentContext
    {
        private IPaymentStrategy _strategy;

        public void SetStrategy(IPaymentStrategy strategy)
        {
            _strategy = strategy;
        }

        public void ExecutePayment(double amount)
        {
            if (_strategy == null)
            {
                Console.WriteLine("Способ оплаты не выбран!");
            }
            else
            {
                _strategy.Pay(amount);
            }
        }
    }

    // OBSERVER PATTERN (Курс валют)

    // 1. Интерфейс наблюдателя
    public interface IObserver
    {
        void Update(string currency, double rate);
    }

    // 2. Интерфейс субъекта
    public interface ISubject
    {
        void AddObserver(IObserver observer);
        void RemoveObserver(IObserver observer);
        void NotifyObservers(string currency, double rate);
    }

    // 3. Конкретный субъект
    public class CurrencyExchange : ISubject
    {
        private List<IObserver> _observers = new List<IObserver>();

        public void AddObserver(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyObservers(string currency, double rate)
        {
            foreach (var observer in _observers)
            {
                observer.Update(currency, rate);
            }
        }

        public void SetRate(string currency, double rate)
        {
            Console.WriteLine($"\nКурс изменился: {currency} = {rate}");
            NotifyObservers(currency, rate);
        }
    }

    // 4. Наблюдатели

    public class MobileApp : IObserver
    {
        public void Update(string currency, double rate)
        {
            Console.WriteLine($"Mobile App получил обновление: {currency} = {rate}");
        }
    }

    public class WebSite : IObserver
    {
        public void Update(string currency, double rate)
        {
            Console.WriteLine($"WebSite обновил курс: {currency} = {rate}");
        }
    }

    public class Investor : IObserver
    {
        public void Update(string currency, double rate)
        {
            if (rate > 500)
                Console.WriteLine("Investor: курс высокий — продаем!");
            else
                Console.WriteLine("Investor: курс низкий — покупаем!");
        }
    }

    // MAIN
    class Program
    {
        static void Main(string[] args)
        {
            // STRATEGY

            Console.WriteLine("===== STRATEGY PATTERN (Оплата) =====\n");

            PaymentContext context = new PaymentContext();

            Console.WriteLine("Выберите способ оплаты:");
            Console.WriteLine("1 - Банковская карта");
            Console.WriteLine("2 - PayPal");
            Console.WriteLine("3 - Криптовалюта");

            int choice = int.Parse(Console.ReadLine());

            Console.Write("Введите сумму: ");
            double amount = double.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.Write("Введите номер карты: ");
                    context.SetStrategy(new CreditCardPayment(Console.ReadLine()));
                    break;

                case 2:
                    Console.Write("Введите email PayPal: ");
                    context.SetStrategy(new PayPalPayment(Console.ReadLine()));
                    break;

                case 3:
                    Console.Write("Введите адрес кошелька: ");
                    context.SetStrategy(new CryptoPayment(Console.ReadLine()));
                    break;

                default:
                    Console.WriteLine("Неверный выбор!");
                    return;
            }

            context.ExecutePayment(amount);


            // OBSERVER

            Console.WriteLine("\n===== OBSERVER PATTERN (Курс валют) =====");

            CurrencyExchange exchange = new CurrencyExchange();

            IObserver mobile = new MobileApp();
            IObserver website = new WebSite();
            IObserver investor = new Investor();

            exchange.AddObserver(mobile);
            exchange.AddObserver(website);
            exchange.AddObserver(investor);

            exchange.SetRate("USD", 480);

            exchange.RemoveObserver(website);

            exchange.SetRate("USD", 520);

            Console.ReadLine();
        }
    }
}