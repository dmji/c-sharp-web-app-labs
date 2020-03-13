using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;

namespace SocketTcpClient {
    class Program {
        static int port = 8805;                                                                                 // порт сервера
        static string address = "127.0.0.1";                                                                // адрес сервера
        //static string address = "192.0.120.9";                                                                // адрес сервера

        public class asSoc
        {
            Socket handler;

            public asSoc(IPEndPoint e)
            {
                handler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                handler.Connect(e);
            }

            public void Sender()
            {
                while (handler.Connected)
                {
                    //Console.Write("\n>> ");
                    string message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    if (message == "-close")
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    handler.Send(data);
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
                    if(builder.ToString().Length>0)
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

            asSoc socket = null;
            //while (ENDCHAR == 'R' || ENDCHAR == 'r' || ENDCHAR == 'к' || ENDCHAR == 'К')            {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                socket = new asSoc(ipPoint);
                Console.Write("Connected to server: " + ipPoint.ToString() + "\n");
                Thread t1 = new Thread(socket.Sender);
                t1.Start();
                //listener
                Thread t2 = new Thread(socket.Listener);
                t2.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            /*
                Console.WriteLine("\nSocet closed. Press 'R' to reconnect or any key to exit.\n");
                ENDCHAR = Console.ReadKey().KeyChar;
                Console.WriteLine("\n");
            */
        }
    }
}