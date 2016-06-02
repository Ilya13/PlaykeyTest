using PlaykeyServer.Server;
using System;
using System.Configuration;

namespace PlaykeyServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Инициализация сервера.");
            try
            {
                var port = int.Parse(ConfigurationManager.AppSettings.Get("Port"));
                Console.WriteLine("Порт: " + port);

                var serverBean = Type.GetType(ConfigurationManager.AppSettings.Get("ServerBean"));
                if (serverBean != null && serverBean.IsSubclassOf(typeof(TcpServer)))
                {
                    var server = (TcpServer) Activator.CreateInstance(serverBean, port);
                    server.Start();
                    Console.WriteLine("Сервер успешно запущен.");
                }
                else
                {
                    Console.WriteLine("Неверно задан класс сервера. Класс должен быть наследован от TcpServer.");
                }
                Console.ReadLine();
            } catch (ArgumentNullException)
            {
                Console.WriteLine("Задайте порт в настройках приложения.");
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
