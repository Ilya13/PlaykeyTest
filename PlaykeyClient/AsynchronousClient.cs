using PlaykeyClient.Handlers;
using System;
using System.Net;
using System.Net.Sockets;
using PlaykeyCommon;

namespace PlaykeyClient
{
    internal class AsynchronousClient
    {   
        private bool _isConnected;
        private IClientActionsHandler _handler;
        private Socket _client;
        
        internal void AddActionsHandler(IClientActionsHandler handler)
        {
            _handler = handler;
        }

        public void StartClient(string host, int port)
        {
            try
            {
                var ipHostInfo = Dns.GetHostEntry(host);
                var remoteEp = new IPEndPoint(ipHostInfo.AddressList[0], port);
                var client = new Socket(remoteEp.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                client.BeginConnect(remoteEp, ConnectCallback, client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _client = (Socket)ar.AsyncState;
                _client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", _client.RemoteEndPoint);

                _isConnected = true;

                Receive();
                _handler?.OnConnected();
            }
            catch (Exception e)
            {
                _handler?.OnError(e.Message);
            }
        }

        private void Receive()
        {
            try
            {
                var info = new SocketConnectionInfo(_client);
                _client.BeginReceive(info.Buffer, 0, ConnectionInfo.BufferSize, 0, ReceiveCallback, info);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                // Получаем информацию о текущем соединении
                var info = (SocketConnectionInfo)result.AsyncState;
                var message = info.Read(result);

                if (message != null)
                {
                    _handler?.OnRecieved(message);
                }

                // Получаем следующий пакет данных
                info.Socket.BeginReceive(info.Buffer, 0, ConnectionInfo.BufferSize, SocketFlags.None, ReceiveCallback, info);
            }
            catch (Exception e)
            {
                _client?.Close();
                _handler?.OnDisconnected();
                Console.WriteLine(e.ToString());
            }
        }

        public void Send(string data)
        {
            if (!_isConnected) return;
            
            var byteData = ConnectionInfo.PrepareToSend(data);
            _client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, _client);
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
