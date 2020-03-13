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

        static void Main(string[] args) {
            char ENDCHAR = 'R';
            
            IPHostEntry host1 = Dns.GetHostEntry("127.0.0.1");
                Console.WriteLine(host1.HostName);
            foreach (IPAddress ip in host1.AddressList)
                Console.WriteLine(ip.ToString());
                
            while (ENDCHAR == 'R' || ENDCHAR == 'r' || ENDCHAR == 'к' || ENDCHAR == 'К') {
                try {

                    IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(ipPoint);                                                                        // подключаемся к удаленному хосту
                    Console.Write("Connected to server: " + ipPoint.ToString());
                    while (socket.Connected == true) {
                        //if(socket.re)
                        Console.Write("\nEnter message:");
                        string message = Console.ReadLine();
                        byte[] data = Encoding.Unicode.GetBytes(message);
                        socket.Send(data);

                        data = new byte[256]; // буфер для ответа
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0; // количество полученных байт
                        do {
                            bytes = socket.Receive(data, data.Length, 0);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (socket.Available > 0);
                        Console.WriteLine("Server feedback: " + builder.ToString());
                        if (message == "-close") {
                            // закрываем сокет
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                            Console.WriteLine("Socet closed. Press 'R' to reconnect or any key to exit.\n");
                            ENDCHAR = Console.ReadKey().KeyChar;
                            Console.WriteLine("\n");
                        }
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}