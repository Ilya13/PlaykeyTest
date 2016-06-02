using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlaykeyCommon
{
    public class ThreadConnectionInfo : ConnectionInfo
    {
        public Socket Socket { get; }
        public Thread Thread { get; }
        
        public ThreadConnectionInfo(Socket socket, ParameterizedThreadStart processConnection)
        {
            Socket = socket;
            Thread = new Thread(processConnection) { IsBackground = true };
        }

        public string Read()
        {
            return Read(Socket.Receive(Buffer));
        }
    }
}
