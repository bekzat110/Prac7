using System;
using System.Collections.Generic;
using System.Linq;

namespace Module07FullVersion
{
    // ===================== COMMAND =======================

    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    #region Devices

    public class Light
    {
        public void On() => Console.WriteLine("Light ON");
        public void Off() => Console.WriteLine("Light OFF");
    }

    public class TV
    {
        public void On() => Console.WriteLine("TV ON");
        public void Off() => Console.WriteLine("TV OFF");
    }

    public class AirConditioner
    {
        public void On() => Console.WriteLine("AC ON");
        public void Off() => Console.WriteLine("AC OFF");
    }

    public class SmartCurtains
    {
        public void Open() => Console.WriteLine("Curtains OPENED");
        public void Close() => Console.WriteLine("Curtains CLOSED");
    }

    public class MusicPlayer
    {
        public void Play() => Console.WriteLine("Music PLAYING");
        public void Stop() => Console.WriteLine("Music STOPPED");
    }

    #endregion

    #region Commands

    public class LightOnCommand : ICommand
    {
        private Light _light;
        public LightOnCommand(Light light) => _light = light;
        public void Execute() => _light.On();
        public void Undo() => _light.Off();
    }

    public class LightOffCommand : ICommand
    {
        private Light _light;
        public LightOffCommand(Light light) => _light = light;
        public void Execute() => _light.Off();
        public void Undo() => _light.On();
    }

    public class TVOnCommand : ICommand
    {
        private TV _tv;
        public TVOnCommand(TV tv) => _tv = tv;
        public void Execute() => _tv.On();
        public void Undo() => _tv.Off();
    }

    public class CurtainsOpenCommand : ICommand
    {
        private SmartCurtains _curtains;
        public CurtainsOpenCommand(SmartCurtains c) => _curtains = c;
        public void Execute() => _curtains.Open();
        public void Undo() => _curtains.Close();
    }

    public class MusicPlayCommand : ICommand
    {
        private MusicPlayer _player;
        public MusicPlayCommand(MusicPlayer p) => _player = p;
        public void Execute() => _player.Play();
        public void Undo() => _player.Stop();
    }

    public class MacroCommand : ICommand
    {
        private List<ICommand> _commands;
        public MacroCommand(List<ICommand> commands) => _commands = commands;

        public void Execute()
        {
            foreach (var cmd in _commands)
                cmd.Execute();
        }

        public void Undo()
        {
            foreach (var cmd in Enumerable.Reverse(_commands))
                cmd.Undo();
        }
    }

    #endregion

    public class RemoteControl
    {
        private Dictionary<int, ICommand> _slots = new();
        private Stack<ICommand> _history = new();
        private Stack<ICommand> _redo = new();

        private bool _recording = false;
        private List<ICommand> _macroBuffer = new();

        public void SetCommand(int slot, ICommand command)
        {
            _slots[slot] = command;
        }

        public void PressButton(int slot)
        {
            if (!_slots.ContainsKey(slot))
            {
                Console.WriteLine("Slot EMPTY!");
                return;
            }

            var command = _slots[slot];
            command.Execute();
            _history.Push(command);
            _redo.Clear();

            if (_recording)
                _macroBuffer.Add(command);
        }

        public void Undo()
        {
            if (_history.Count == 0) return;
            var cmd = _history.Pop();
            cmd.Undo();
            _redo.Push(cmd);
        }

        public void Redo()
        {
            if (_redo.Count == 0) return;
            var cmd = _redo.Pop();
            cmd.Execute();
            _history.Push(cmd);
        }

        public void StartRecording()
        {
            _recording = true;
            _macroBuffer.Clear();
            Console.WriteLine("Recording macro...");
        }

        public MacroCommand StopRecording()
        {
            _recording = false;
            Console.WriteLine("Macro saved!");
            return new MacroCommand(new List<ICommand>(_macroBuffer));
        }
    }

    // ================ TEMPLATE METHOD ====================

    public abstract class ReportGenerator
    {
        public void GenerateReport()
        {
            Log("Start report generation");
            FetchData();
            FormatData();
            CreateHeader();

            if (CustomerWantsSave())
                SaveReport();
            else
                SendEmail();

            Log("Report finished\n");
        }

        protected abstract void FetchData();
        protected abstract void FormatData();
        protected abstract void CreateHeader();
        protected abstract void SaveReport();

        protected virtual bool CustomerWantsSave()
        {
            while (true)
            {
                Console.WriteLine("Save report to file? (y/n)");
                string input = Console.ReadLine()?.ToLower();

                if (input == "y") return true;
                if (input == "n") return false;

                Console.WriteLine("Invalid input. Try again.");
            }
        }

        protected virtual void SendEmail()
        {
            Console.WriteLine("Report sent via Email.");
        }

        protected void Log(string message)
        {
            Console.WriteLine("[LOG] " + message);
        }
    }

    public class PdfReport : ReportGenerator
    {
        protected override void FetchData() => Console.WriteLine("Fetch PDF data");
        protected override void FormatData() => Console.WriteLine("Format PDF");
        protected override void CreateHeader() => Console.WriteLine("Create PDF header");
        protected override void SaveReport() => Console.WriteLine("Save PDF file");
    }

    public class ExcelReport : ReportGenerator
    {
        protected override void FetchData() => Console.WriteLine("Fetch Excel data");
        protected override void FormatData() => Console.WriteLine("Format Excel");
        protected override void CreateHeader() => Console.WriteLine("Create Excel header");
        protected override void SaveReport() => Console.WriteLine("Save Excel file (.xlsx)");
    }

    public class HtmlReport : ReportGenerator
    {
        protected override void FetchData() => Console.WriteLine("Fetch HTML data");
        protected override void FormatData() => Console.WriteLine("Format HTML");
        protected override void CreateHeader() => Console.WriteLine("Create HTML header");
        protected override void SaveReport() => Console.WriteLine("Save HTML file");
    }

    public class CsvReport : ReportGenerator
    {
        protected override void FetchData() => Console.WriteLine("Fetch CSV data");
        protected override void FormatData() => Console.WriteLine("Format CSV");
        protected override void CreateHeader() => Console.WriteLine("Create CSV header");
        protected override void SaveReport() => Console.WriteLine("Save CSV file");
    }

    // ==================== MEDIATOR =======================

    public interface IMediator
    {
        void RegisterUser(User user, string channel);
        void SendMessage(string channel, User sender, string message);
        void SendPrivate(User sender, string receiverName, string message);
        void BlockUser(string channel, string userName);
    }

    public class ChatMediator : IMediator
    {
        private Dictionary<string, List<User>> _channels = new();
        private Dictionary<string, HashSet<string>> _blocked = new();

        public void RegisterUser(User user, string channel)
        {
            if (!_channels.ContainsKey(channel))
            {
                _channels[channel] = new List<User>();
                _blocked[channel] = new HashSet<string>();
            }

            _channels[channel].Add(user);
            user.CurrentChannel = channel;

            Broadcast(channel, "SYSTEM", $"{user.Name} joined channel");
        }

        public void SendMessage(string channel, User sender, string message)
        {
            if (!_channels.ContainsKey(channel))
            {
                Console.WriteLine("Channel not found!");
                return;
            }

            if (!_channels[channel].Contains(sender))
            {
                Console.WriteLine("User not in this channel!");
                return;
            }

            if (_blocked[channel].Contains(sender.Name))
            {
                Console.WriteLine("User is BLOCKED!");
                return;
            }

            Broadcast(channel, sender.Name, message);
        }

        public void SendPrivate(User sender, string receiverName, string message)
        {
            var receiver = _channels.Values
                .SelectMany(u => u)
                .FirstOrDefault(u => u.Name == receiverName);

            if (receiver == null)
            {
                Console.WriteLine("User not found!");
                return;
            }

            receiver.Receive($"[PRIVATE] {sender.Name}: {message}");
        }

        public void BlockUser(string channel, string userName)
        {
            if (_blocked.ContainsKey(channel))
            {
                _blocked[channel].Add(userName);
                Console.WriteLine($"{userName} blocked in {channel}");
            }
        }

        private void Broadcast(string channel, string sender, string message)
        {
            foreach (var user in _channels[channel])
                user.Receive($"{sender}: {message}");
        }
    }

    public class User
    {
        public string Name { get; }
        public string CurrentChannel { get; set; }

        public User(string name)
        {
            Name = name;
        }

        public void Receive(string message)
        {
            Console.WriteLine($"[{Name}] {message}");
        }
    }

    // ======================== MAIN =======================

    class Program
    {
        static void Main()
        {
            Console.WriteLine("===== COMMAND =====");

            var remote = new RemoteControl();
            var light = new Light();
            var tv = new TV();
            var curtains = new SmartCurtains();
            var music = new MusicPlayer();

            remote.SetCommand(1, new LightOnCommand(light));
            remote.SetCommand(2, new TVOnCommand(tv));
            remote.SetCommand(3, new CurtainsOpenCommand(curtains));
            remote.SetCommand(4, new MusicPlayCommand(music));

            remote.PressButton(1);
            remote.Undo();
            remote.Redo();

            remote.StartRecording();
            remote.PressButton(1);
            remote.PressButton(4);
            var macro = remote.StopRecording();

            remote.SetCommand(5, macro);
            remote.PressButton(5);

            Console.WriteLine("\n===== TEMPLATE =====");
            ReportGenerator report = new PdfReport();
            report.GenerateReport();

            Console.WriteLine("\n===== MEDIATOR =====");
            var mediator = new ChatMediator();
            var u1 = new User("Bekzat");
            var u2 = new User("Ali");

            mediator.RegisterUser(u1, "General");
            mediator.RegisterUser(u2, "General");

            mediator.SendMessage("General", u1, "Hello everyone!");
            mediator.SendPrivate(u1, "Ali", "Private hi!");

            mediator.BlockUser("General", "Ali");
            mediator.SendMessage("General", u2, "Test after block");
        }
    }
}