using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using PlaykeyCommon;

namespace PlaykeyServer.Server
{
    internal class AsynchronousIoServer : TcpServer
    {
        private readonly List<SocketConnectionInfo> _connections;

        public AsynchronousIoServer(int port) : base(port)
        {
            _connections = new List<SocketConnectionInfo>();
        }

        protected override void Run()
        {
            for (var i = 0; i < 10; i++)
            {
                ServerSocket.BeginAccept(AcceptCallback, ServerSocket);
            }
        }

        private void AcceptCallback(IAsyncResult result)
        {
            SocketConnectionInfo connection = null;
            try
            {
                var s = (Socket)result.AsyncState;
                connection = new SocketConnectionInfo(s.EndAccept(result));
                lock (_connections) _connections.Add(connection);
                
                connection.Socket.BeginReceive(connection.Buffer, 0, ConnectionInfo.BufferSize, 
                    SocketFlags.None, ReceiveCallback, connection);
                ServerSocket.BeginAccept(AcceptCallback, result.AsyncState);
            }
            catch (SocketException e)
            {
                CloseConnection(connection);
                Console.WriteLine("Socket exception: " + e.SocketErrorCode);
            }
            catch (Exception e)
            {
                CloseConnection(connection);
                Console.WriteLine("Exception: " + e);
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            var connection = (SocketConnectionInfo)result.AsyncState;
            try
            {
                var message = connection.Read(result);

                if (message != null)
                {
                    if (message.Equals(ConnectionInfo.GetLogCommand))
                    {
                        var byteData = ConnectionInfo.PrepareToSend(ConnectionInfo.GetLogCommand + Logger.GetLog());
                        connection.Socket.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, connection);
                    }
                    else
                    {
                        Logger.Log(message);
                        lock (_connections)
                        {
                            var byteData = ConnectionInfo.PrepareToSend(message);
                            foreach (var conn in _connections.Where(conn => !connection.Equals(conn)))
                            {
                                conn.Socket.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, conn);
                            }
                        }
                    }
                }
                connection.Socket.BeginReceive(connection.Buffer, 0, ConnectionInfo.BufferSize,
                        SocketFlags.None, ReceiveCallback, connection);

            }
            catch (SocketException exc)
            {
                CloseConnection(connection);
                Console.WriteLine("Socket exception: " + exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                CloseConnection(connection);
                Console.WriteLine("Exception: " + exc);
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

        private void CloseConnection(SocketConnectionInfo connection)
        {
            if (connection == null) return;
            connection.Socket.Close();
            lock (_connections) _connections.Remove(connection);
        }
    }
}
