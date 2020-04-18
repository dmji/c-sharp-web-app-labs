using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;

namespace SocketTcpClient
{
    class Program
    {
        static int port = 8805;                                                                                 // порт сервера
        static string address = "127.0.0.1";                                                                // адрес сервера
        //static string address = "192.0.120.9";                                                                // адрес сервера

        public class chat_socket
        {
            Socket handler;
            public Thread t1 = null;
            public Thread t2 = null;

            public chat_socket(IPEndPoint e)
            {
                handler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                handler.Connect(e);
                t1 = new Thread(Sender);
                t2 = new Thread(Listener);
                t1.Start();
                t2.Start();
            }

            public void Sender()
            {
                string msg;
                while (handler.Connected)
                {
                    msg=Console.ReadLine();
                    if(handler.Connected) handler.Send(Encoding.Unicode.GetBytes(msg));
                }
            }
            public void Listener()
            {
                while (handler.Connected)
                {
                    byte[] data = new byte[256]; // буфер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байт
                    do
                    {
                        bytes = handler.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (handler.Available > 0);
                    if(builder.ToString()== "dc785b5cd340896d05ad96b6b4f876bd") //to close
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        t1.Abort();
                        handler.Close();
                    }
                    else if (builder.ToString().Length > 0)
                        Console.WriteLine("<< " + builder.ToString());
                    Thread.Sleep(100);
                }
            }
        }

        static void Main(string[] args)
        {
            char ENDCHAR = 'R';

            IPHostEntry host1 = Dns.GetHostEntry("127.0.0.1");
            Console.WriteLine(host1.HostName);
            foreach (IPAddress ip in host1.AddressList)
                Console.WriteLine(ip.ToString());

            chat_socket socket = null;
            while (ENDCHAR == 'R' || ENDCHAR == 'r' || ENDCHAR == 'к' || ENDCHAR == 'К')
            {
                try
                {
                    IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                    socket = new chat_socket(ipPoint);
                    Console.Write("Connected to server: " + ipPoint.ToString() + "\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n!!! "+ex.Message + "\n");
                }
                socket.t1.Join();
                socket.t2.Join();
                Console.WriteLine("\nSocet closed. Press 'R' to reconnect or any key to exit.\n");
                ENDCHAR = Console.ReadKey().KeyChar;
                Console.WriteLine("\n");

            }
        }
    }
}