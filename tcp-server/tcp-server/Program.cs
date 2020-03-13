using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketTcpServer {
    class Program {
        static int port = 8805; // порт для приема входящих запросов

        public class asSoc {
            Socket handler;
            string name;
            static int enter_count=1;
            int id;

            public asSoc(Socket e)
            {
                handler = e;
                name = null;
                id = enter_count;
                enter_count++;
            }

            public void AcceptCallback() {
                Console.WriteLine(">> Client #" + id + " connected first time");
                byte[] data = new byte[256];
                int bytes;
                StringBuilder builder= new StringBuilder();
                data = Encoding.Unicode.GetBytes("Send name first.");
                handler.Send(data);
                bytes = 0; // количество полученных байтов
                data = new byte[256]; // буфер для получаемых данных
                Console.WriteLine(">> Client #" + id + " registred as: " + name);
                do
                {
                    bytes = handler.Receive(data);                                                                   // получаем сообщение
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (handler.Available > 0);
                name = builder.ToString();
                handler.Send(Encoding.Unicode.GetBytes("Name set as \'" + name + "\'.\n"));
                try {
                    while (handler!=null) {                        builder = new StringBuilder();
                        bytes = 0; // количество полученных байтов
                        data = new byte[256]; // буфер для получаемых данных
                        do {
                            bytes = handler.Receive(data);                                                                   // получаем сообщение
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (handler.Available > 0);
                        Console.WriteLine(DateTime.Now.ToShortTimeString() + " " + name + " : "  + builder.ToString());                    //вывод сообщения на сервере
                        if (builder.ToString() == "-close") {
                            // закрываем сокет
                            string message = "@Connection closed.";
                            data = Encoding.Unicode.GetBytes(message);
                            handler.Send(data);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            handler = null;
                        }
                        else {
                            string message = "message back: # " + builder.ToString() + " #";
                            data = Encoding.Unicode.GetBytes(message);
                            handler.Send(data);
                        }
                    }
                }
                catch (Exception e) {
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
                Thread t = new Thread(soc.AcceptCallback);
                t.Start();
            }
        }
    }
}