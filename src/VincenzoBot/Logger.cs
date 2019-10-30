using System;
using Moq;

namespace VincenzoBot
{
    public class Logger :ILogger
    {
        public void Log(string msg, [System.Runtime.CompilerServices.CallerMemberName] string caller="")
        {
            if (caller.Equals("Discord"))
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            if (msg.Contains("EXCEPTION: "))
                Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{caller}: {msg}");
            Console.ResetColor();
        }

        public static implicit operator Logger(Mock<Logger> v)
        {
            return v;
        }
    }
}
