using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketTcpServer {
    class Program {
        static int port = 8805; // порт для приема входящих запросов

        public class chat_socket
        {
            static System.Collections.Generic.List<Socket> msg = new System.Collections.Generic.List<Socket>();
            Socket handler;
            string name;
            Thread t = null;

            public chat_socket(Socket e)
            {
                handler = e;
                name = null;
                t = new Thread(Registration);
                t.Start();
                Console.WriteLine( "####" +handler.RemoteEndPoint.ToString() );
            }

            private void exDestroy()
            {
                if (handler != null)
                {
                    lock (handler)
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        handler = null;
                        lock (msg)
                            if(msg.FindIndex(h => h == handler)>=0)
                                msg.RemoveAt(msg.FindIndex(h => h == handler));
                    }
                }
            }

            public void Registration()
            {
                Console.WriteLine(">> Client #" + id + " connected first time");
                byte[] data = new byte[256]; // буфер для получаемых данных
                int bytes = 0; // количество полученных байтов
                StringBuilder builder = new StringBuilder();
                data = Encoding.Unicode.GetBytes("Send name first.");
                try
                {
                    handler.Send(data);
                    do
                    {
                        bytes = handler.Receive(data);                                                                   // получаем сообщение
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (handler.Available > 0);
                    name = builder.ToString();
                    handler.Send(Encoding.Unicode.GetBytes("Name set as \'" + name + "\'.\n"));
                    Console.WriteLine(">> Client #" + id + " registred as: " + name);
                }
                catch (Exception e)
                {
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + "@Connection breaked.");
                    exDestroy();
                }
                if (handler != null)
                {
                    t = new Thread(Chat);
                    t.Start();
                    msg.Add(handler);
                }
            }

            public void Chat()
            {
                StringBuilder builder = new StringBuilder();
                byte[] data = new byte[256];
                int bytes;
                try                
                {
                    while (handler!=null)
                    {
                        builder = new StringBuilder();
                        bytes = 0; // количество полученных байтов
                        data = new byte[256]; // буфер для получаемых данных
                        do
                        {
                            bytes = handler.Receive(data);                                                                   // получаем сообщение
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                            Thread.Sleep(100);
                        } while (handler.Available > 0);
                        string message = builder.ToString();
                        string lmsg = DateTime.Now.ToShortTimeString() + " " + name + " : " + builder.ToString();
                        Console.WriteLine(lmsg);                    //вывод сообщения на сервере
                        if (builder.ToString() == "-close")
                        {
                            // закрываем сокет
                            handler.Send(Encoding.Unicode.GetBytes("dc785b5cd340896d05ad96b6b4f876bd"));
                            exDestroy();
                        }
                        else
                        {
                            foreach(Socket s in msg)
                            {
                                if(s.Connected)
                                    s.Send(Encoding.Unicode.GetBytes(lmsg));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + "@Connection breaked.");
                    exDestroy();
                }
            }
        }
                

        static void Main(string[] args) 
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, port);                                                   // получаем адреса для запуска сокета
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);          // создаем сокет
            listenSocket.Bind(ipPoint);                                                                             // связываем сокет с локальной точкой, по которой будем принимать данные
            listenSocket.Listen(10);                                                                                // начинаем прослушивание
            Console.WriteLine("Server is up. Wait for clients...");
            while (true)
                new chat_socket(listenSocket.Accept());
        }
    }
}