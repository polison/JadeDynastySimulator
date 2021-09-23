using NetWorkLibrary;
using System;

namespace JadeDynastySimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("欢迎使用诛仙1760模拟器！");

            Log logger = new Log();
            WorldSocketManager socketManager = new WorldSocketManager(typeof(WorldSocketClass), logger);
            socketManager.OpenConnection(29000);
            while (Console.Read() != 'q')
            {

            }
            socketManager.CloseConnection();
        }
    }

    public class Log : ILog
    {
        public void Error(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void Message(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(format, args);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void Warning(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(format, args);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
