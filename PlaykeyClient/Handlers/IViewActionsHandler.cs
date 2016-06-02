namespace PlaykeyClient.Handlers
{
    public interface IViewActionsHandler
    {
        void OnGetLog();
        void OnSendMesage(string message);
        void Connect(string text, int port);
    }
}
