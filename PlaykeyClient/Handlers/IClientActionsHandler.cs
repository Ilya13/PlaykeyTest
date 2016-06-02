namespace PlaykeyClient.Handlers
{
    public interface IClientActionsHandler
    {
        void OnConnected();
        void OnDisconnected();
        void OnRecieved(string message);
        void OnError(string error);
    }
}
