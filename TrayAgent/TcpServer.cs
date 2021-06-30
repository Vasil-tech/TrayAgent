using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrayAgent
{
    public struct TcpServer //Структура данных тестируемого сервера
    {   
        public string host;
        public int port;
        public bool succ;
        public string capt;

        public TcpServer(string host, int port, bool succ, string capt)
        {
            this.host = host;
            this.port = port;
            this.succ = succ;
            this.capt = capt;
        }
    }
}
