using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;

namespace WEB_INFO
{
    class Program
    {
        static void Main(string[] args)
        {
            string address = "127.0.0.1";
            int port = 8805;
            DnsEndPoint dnsT = new DnsEndPoint(address, 8805);
            Console.WriteLine("AddressFamily: " + dnsT.AddressFamily.ToString() + "\nHost: " + dnsT.Host + "\nPort: " + dnsT.Port);
            Console.WriteLine();

          
            System.Text.ASCIIEncoding ASCII = new System.Text.ASCIIEncoding();
            IPHostEntry heserver =Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress curAdd in heserver.AddressList)
            {
                Console.WriteLine("AddressFamily: " + curAdd.AddressFamily.ToString());

                // Display the ScopeId property in case of IPV6 addresses.
                if (curAdd.AddressFamily.ToString() == ProtocolFamily.InterNetworkV6.ToString())
                    Console.WriteLine("Scope Id: " + curAdd.ScopeId.ToString());
                Console.WriteLine("Address: " + curAdd.ToString());

                // Display the server IP address in byte format.
                Console.Write("AddressBytes: ");
                Byte[] bytes = curAdd.GetAddressBytes();
                for (int i = 0; i < bytes.Length; i++)
                    Console.Write(bytes[i]);

                Console.WriteLine("\r\n");
            }

            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine(soc.ToString());

        }
    }
}
