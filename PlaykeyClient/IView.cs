using PlaykeyClient.Handlers;

namespace PlaykeyClient
{
    internal interface IView
    {
        void SetActionsHandler(IViewActionsHandler handler);
        void PrintMessage(string message);
        void PrintLog(string log);
        void ShowMessage(string message);
        void OnConnected();
        void OnDisconnected();
    }
}
