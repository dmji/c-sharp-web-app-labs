using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketTcpServer {
    class Program {
        static int port = 8805; // порт для приема входящих запросов

        public class asSoc
        {
            static System.Collections.Generic.List<string> msg = new System.Collections.Generic.List<string>();
            Socket handler;
            string name;
            static int enter_count = 1;
            int id;
            int msg_id;

            public asSoc(Socket e)
            {
                msg_id = 0;
                handler = e;
                name = null;
                id = enter_count;
                enter_count++;
                Console.WriteLine( "####" +handler.RemoteEndPoint.ToString() );
            }

            public void Registration()
            {
                Console.WriteLine(">> Client #" + id + " connected first time");
                byte[] data = new byte[256]; // буфер для получаемых данных
                int bytes=0; // количество полученных байтов
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
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    handler = null;
                }
                Thread t1 = new Thread(this.Listen);
                t1.Start();
                Thread t2 = new Thread(this.Sender);
                t2.Start();
            }

            public void Listen()
            {
                StringBuilder builder = new StringBuilder();
                byte[] data = new byte[256];
                int bytes;
                try                
                {
                    while (handler != null)
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
                        string lmsg = DateTime.Now.ToShortTimeString() + " " + name + " : " + builder.ToString();
                        Console.WriteLine(lmsg);                    //вывод сообщения на сервере
                        msg.Add(lmsg);
                        if (builder.ToString() == "-close")
                        {
                            // закрываем сокет
                            string message = "@Connection closed.";
                            data = Encoding.Unicode.GetBytes(message);
                            handler.Send(data);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            handler = null;
                        }
                        else
                        {
                            string message = builder.ToString();
                            data = Encoding.Unicode.GetBytes(message);
                            handler.Send(data);
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + "@Connection breaked.");
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    handler = null;
                }
               
            }

            public void Sender()
            {
                StringBuilder builder = new StringBuilder();
                byte[] data = new byte[256];
                int bytes;
                try
                {
                    while (handler != null)
                    {
                        while (msg.Count > msg_id)
                        {
                            data = Encoding.Unicode.GetBytes(msg[msg_id++]);
                            handler.SendTo(data, SocketFlags.None, handler.LocalEndPoint);
                        }
                        Thread.Sleep(100);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + "@Connection breaked.");
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    handler = null;
                }
            }
        }
                

        static void Main(string[] args) {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, port);                                                   // получаем адреса для запуска сокета
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);          // создаем сокет
            listenSocket.Bind(ipPoint);                                                                             // связываем сокет с локальной точкой, по которой будем принимать данные
            listenSocket.Listen(10);                                                                                // начинаем прослушивание
            Console.WriteLine("Server is up. Wait for clients...");

            while (true) {
                asSoc soc = new asSoc(listenSocket.Accept());
                Thread t = new Thread(soc.Registration);
                t.Start();
               
            }
        }
    }
}