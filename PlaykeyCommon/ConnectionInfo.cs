using System;
using System.Text;

namespace PlaykeyCommon
{
    public class ConnectionInfo
    {
        private static int _count;

        // Размер пакета
        public const int BufferSize = 256;
        // Символ обозначающий конец строки
        public const string Eof = "<EOF>";
        // Символ обозначающий конец строки
        public const string GetLogCommand = "<GETLOG>";

        // Имя соединения
        public int Id { get; }
        // Данные пакета
        public byte[] Buffer { get; set; }
        // Строковое представление
        private string _message;

        public ConnectionInfo()
        {
            Id = ++_count;
            Clear();
        }

        private void Clear()
        {
            Buffer = new byte[BufferSize];
            _message = string.Empty;
        }

        private int Append(int bytesRead)
        {
            _message += Encoding.Unicode.GetString(Buffer, 0, bytesRead);
            return _message.IndexOf("<EOF>", StringComparison.Ordinal);
        }

        public string Read(int bytesRead)
        {
            string result = null;

            if (bytesRead > 0)
            {
                var last = Append(bytesRead);
                if (last > -1)
                {
                    result = _message.Substring(0, last);
                    Clear();
                }
            }
            else if (!string.IsNullOrEmpty(_message))
            {
                result = _message;
                Clear();
            }
            return result;
        }

        public static byte[] PrepareToSend(string message)
        {
            if (message.IndexOf(Eof, StringComparison.Ordinal) == -1)
            {
                message += Eof;
            }
            return Encoding.Unicode.GetBytes(message);
        }

        public override string ToString()
        {
            return "Клиент " + Id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var info = (ConnectionInfo)obj;
            return Id == info.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
