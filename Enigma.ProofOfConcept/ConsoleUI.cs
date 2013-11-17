using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Enigma.Store.Keys;

namespace Enigma.ProofOfConcept
{
    public class ConsoleUI
    {
        private readonly Dictionary<char, IConsoleCommand> _commands;
        private int _nextCommandShortcut;

        public ConsoleUI()
        {
            _commands = new Dictionary<char, IConsoleCommand>();
            _nextCommandShortcut = '1';
        }

        private char GetNextCommandShortcut()
        {
            var shortcut = _nextCommandShortcut;
            if (shortcut == '9')
                _nextCommandShortcut = 'a';
            else
                _nextCommandShortcut++;

            return (char) shortcut;
        }

        public void AddCommand(IConsoleCommand command)
        {
            _commands.Add(GetNextCommandShortcut(), command);
        }

        public void Run()
        {
            WriteHelp();
            var key = ReadKey();
            while (!(key.Modifiers.HasFlag(ConsoleModifiers.Control) && key.KeyChar == 'c')) {
                IConsoleCommand command;
                if (_commands.TryGetValue(key.KeyChar, out command))
                    command.Invoke();
                else
                    WriteHelp();

                key = ReadKey();
            }
        }

        private static ConsoleKeyInfo ReadKey()
        {
            Console.Write("Enter a key: ");
            var key = Console.ReadKey();

            var modifiers = new List<ConsoleModifiers>();
            if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                modifiers.Add(ConsoleModifiers.Control);
            if (key.Modifiers.HasFlag(ConsoleModifiers.Alt))
                modifiers.Add(ConsoleModifiers.Alt);
            if (key.Modifiers.HasFlag(ConsoleModifiers.Shift))
                modifiers.Add(ConsoleModifiers.Shift);

            if (modifiers.Any()) {
                Console.Write(string.Join("+", modifiers));
                Console.Write("+");
            }
            Console.WriteLine(key.Key);
            return key;
        }

        private void WriteHelp()
        {
            foreach (var keyValue in _commands)
                Console.WriteLine("{0} - {1}", keyValue.Key, keyValue.Value.GetType().Name);
            Console.WriteLine("Ctrl+c - Exit application");
        }
    }
}
