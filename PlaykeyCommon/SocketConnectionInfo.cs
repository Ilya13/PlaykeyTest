using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PlaykeyCommon
{
    public class SocketConnectionInfo : ConnectionInfo
    {
        public Socket Socket { get; }

        public SocketConnectionInfo(Socket socket)
        {
            Socket = socket;
        }

        public string Read(IAsyncResult result)
        {
            return Read(Socket.EndReceive(result));
        }
    }
}
