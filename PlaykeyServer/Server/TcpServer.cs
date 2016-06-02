using System;
using System.Net;
using System.Net.Sockets;

namespace PlaykeyServer.Server
{
    internal abstract class TcpServer
    {
        protected Socket ServerSocket;
        protected int Port;

        protected TcpServer(int port) { Port = port; }
        
        public void Start()
        {
            SetupServerSocket();
            Run();
        }

        protected void SetupServerSocket()
        {
            // Получаем информацию о локальном компьютере
            var hostName = Dns.GetHostName();

            Console.WriteLine("Имя сервера: " + hostName + ".");
            var localMachineInfo = Dns.GetHostEntry(hostName);
            var myEndpoint = new IPEndPoint(localMachineInfo.AddressList[0], Port);

            // Создаем сокет, привязываем его к адресу
            // и начинаем прослушивание
            ServerSocket = new Socket(myEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(myEndpoint);
            Console.WriteLine("Запуск прослушивания сокета.");
            ServerSocket.Listen((int)SocketOptionName.MaxConnections);
        }

        protected abstract void Run();
    }
}
