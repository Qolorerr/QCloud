using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.Connection
{
    class SendData
    {
        private string[] IpList;

        public SendData(string[] ipList)
        {
            IpList = ipList;
        }

        public bool SendFile(string link, string name="")
        {
            if (name == "")
            {
                name = Path.GetFileName(link);
            }

            var result = false;
            const int port = 8080;

            foreach (var ip in IpList)
            {
                var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                var data = Encoding.UTF8.GetBytes(name);

                tcpSocket.Connect(tcpEndPoint);
                tcpSocket.Send(data);
                tcpSocket.SendFile(link);

                var buffer = new byte[256];
                var size = 0;
                var answer = new StringBuilder();

                do
                {
                    size = tcpSocket.Receive(buffer);
                    answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (tcpSocket.Available > 0);

                if (answer.ToString() == "True")
                {
                    result = true;
                }

                tcpSocket.Shutdown(SocketShutdown.Both);
                tcpSocket.Close();
            }
            return result;
        }
    }
}
