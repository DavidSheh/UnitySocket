using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace UnitySocketServer
{
    class Program
    {
        private static byte[] result = new byte[1024];
        private const int port = 8088;
        private static string IpStr = "127.0.0.1";
        private static Socket serverSocket;

        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse(IpStr);
            IPEndPoint ip_end_point = new IPEndPoint(ip, port);
            //创建服务器Socket对象，并设置相关属性
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定ip和端口
            serverSocket.Bind(ip_end_point);
            //设置最长的连接请求队列长度
            serverSocket.Listen(10);
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            //在新线程中监听客户端的连接
            Thread thread = new Thread(ClientConnectListen);
            thread.Start();
            Console.ReadLine();
        }

        /// <summary>
        /// 客户端连接请求监听
        /// </summary>
        private static void ClientConnectListen()
        {
            while (true)
            {
                //为新的客户端连接创建一个Socket对象
                Socket clientSocket = serverSocket.Accept();
                Console.WriteLine("客户端{0}成功连接", clientSocket.RemoteEndPoint.ToString());
                //向连接的客户端发送连接成功的数据
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteString("Connected Server!");
                clientSocket.Send(buffer.ToBytes());
                //每个客户端连接创建一个线程来接受该客户端发送的消息
                Thread thread = new Thread(RecieveMessage);
                thread.Start(clientSocket);
            }
        }

        /// <summary>
        /// 接收指定客户端Socket的消息
        /// </summary>
        /// <param name="clientSocket"></param>
        private static void RecieveMessage(object clientSocket)
        {
            Socket mClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    int receiveNumber = mClientSocket.Receive(result);

                    if (receiveNumber <= 0)
                    {
                        continue;
                    }

                    Console.WriteLine("接收客户端{0}消息， 总长度为{1}", mClientSocket.RemoteEndPoint.ToString(), receiveNumber);
                    ByteBuffer buff = new ByteBuffer(result);
                    //数据内容
                    string data = buff.ReadString();
                    Console.WriteLine("消息内容：{0}", data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    mClientSocket.Shutdown(SocketShutdown.Both);
                    mClientSocket.Close();
                    break;
                }
            }
        }
    }
}
