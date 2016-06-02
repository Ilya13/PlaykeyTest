using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using PlaykeyCommon;

namespace PlaykeyServer.Server
{
    internal class ThreadedServer : TcpServer
    {
        private Thread _acceptThread;
        private readonly List<ThreadConnectionInfo> _connections;

        public ThreadedServer(int port) : base(port)
        {
            _connections = new List<ThreadConnectionInfo>();
        }

        protected override void Run()
        {
            _acceptThread = new Thread(AcceptConnections) {IsBackground = true};
            _acceptThread.Start();
        }

        private void AcceptConnections()
        {
            while (true)
            {
                // Принимаем соединение
                var socket = ServerSocket.Accept();
                var connection = new ThreadConnectionInfo(socket, ProcessConnection);

                // Создаем поток для получения данных
                connection.Thread.Start(connection);

                // Сохраняем сокет
                lock (_connections) _connections.Add(connection);
            }
        }

        private void ProcessConnection(object state)
        {
            var connection = (ThreadConnectionInfo)state;
            try
            {
                while (true)
                {
                    var message = connection.Read();
                    if (message == null) break;

                    if (message.Equals(ConnectionInfo.GetLogCommand))
                    {
                        var byteData = ConnectionInfo.PrepareToSend(message+Logger.GetLog());
                        connection.Socket.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, connection);
                    }
                    else
                    {
                        Logger.Log(message);
                        lock (_connections)
                        {
                            var byteData = ConnectionInfo.PrepareToSend(message);
                            foreach (var conn in _connections.Where(conn => conn != connection))
                            {
                                conn.Socket.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, conn);
                            }
                        }
                    }
                }
            }
            catch (SocketException exc)
            {
                Console.WriteLine("Socket exception: " + exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception: " + exc);
            }
            finally
            {
                connection.Socket.Close();
                lock (_connections) _connections.Remove(connection);
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                var info = (SocketConnectionInfo)ar.AsyncState;
                var bytesSent = info.Socket.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
