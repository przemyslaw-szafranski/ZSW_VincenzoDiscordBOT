using System;

namespace VincenzoDiscordBot
{
    class NotificationService
    {
        public void ConsolePrint(string msg)
        {
            Console.WriteLine(msg);
        }
        public void ExceptionConsolePrint(string msg, string memberName ="#404")
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(memberName+"EXCEPTION: " + msg);
            Console.WriteLine("Click any button to continue...");
            Console.ResetColor();
            Console.ReadKey(true);
        }
    }
}
