using PlaykeyClient.Handlers;
using System;
using PlaykeyCommon;

namespace PlaykeyClient
{
    internal class Controller : IViewActionsHandler, IClientActionsHandler
    {
        private readonly IView _view;
        private readonly AsynchronousClient _client;

        public Controller(IView view)
        {
            _view = view;
            _view.SetActionsHandler(this);
            
            try
            {
                _client = new AsynchronousClient();
                _client.AddActionsHandler(this);
            }
            catch (Exception ex)
            {
                _view.ShowMessage(ex.Message);
            }
        }
        
        public void OnGetLog()
        {
            _client.Send(ConnectionInfo.GetLogCommand);
        }

        public void OnSendMesage(string message)
        {
            _client.Send(message);
        }

        public void Connect(string host, int port)
        {
            _client.StartClient(host, port);
        }

        public void OnConnected()
        {
            _view.OnConnected();
        }

        public void OnDisconnected()
        {
            _view?.OnDisconnected();
        }

        public void OnRecieved(string message)
        {
            if (message.StartsWith(ConnectionInfo.GetLogCommand))
            {
                _view.PrintLog(message.Substring(ConnectionInfo.GetLogCommand.Length));
            }
            else
            {
                _view.PrintMessage(message);
            }
        }

        public void OnError(string error)
        {
            _view.ShowMessage(error);
        }
    }
}
