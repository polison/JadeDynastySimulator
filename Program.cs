using NetWorkLibrary;
using System;
using System.Security.Cryptography;
using System.Text;

namespace JadeDynastySimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("欢迎使用诛仙1760模拟器！");

            Log logger = new Log();
            DataBaseManager.Instance.DBHost = "localhost";
            DataBaseManager.Instance.DBUser = "root";
            DataBaseManager.Instance.DBPass = "123456";
            DataBaseManager.Instance.DBName = "element_world";
            DataBaseManager.Instance.DBPort = 3306;
            if (!DataBaseManager.Instance.Initialize(logger))
                return;
            WorldSocketManager socketManager = new WorldSocketManager(typeof(WorldSocket), typeof(WorldPacket), logger);
            socketManager.OpenConnection(29000);

            bool isRun = true;
            while (isRun)
            {
                var key = Console.ReadLine();
                switch (key)
                {
                    case "c":
                        {
                            Console.Write("创建用户,请输入用户名");
                            string username = Console.ReadLine();
                            Console.Write("创建用户,请输入密码");
                            string userpwd = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(userpwd))
                            {
                                var pwd = new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(username + userpwd));
                                if (DataBaseManager.Instance.PExecute("insert into users set uname=\"{0}\",upwd=0x{1};", username, BitConverter.ToString(pwd).Replace("-", "")))
                                {
                                    Console.Write("创建用户成功！");
                                    Console.WriteLine();
                                }
                                else
                                {
                                    Console.Write("创建用户失败，可能用户名已经存在！");
                                    Console.WriteLine();
                                }
                            }
                            else
                            {
                                Console.Write("请正确输入用户名，密码！");
                                Console.WriteLine();
                            }
                        }
                        break;
                    case "q":
                        isRun = false;
                        break;
                }
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
