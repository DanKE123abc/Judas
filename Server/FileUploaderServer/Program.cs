using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
namespace JudasServer;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: FileUploaderServer <hostname> <port>");
            return;
        }

        string hostname = args[0];
        string port = args[1];

        TcpListener server = null;
        try
        {
            IPAddress localAddr = IPAddress.Parse(hostname);

            // 启动服务器
            server = new TcpListener(localAddr, int.Parse(port));
            server.Start();

            // 等待客户端连接
            Console.WriteLine("Waiting for a connection... ");
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Connected!");

            // 接收文件并保存
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[1024];
            int bytesRead = stream.Read(data, 0, data.Length);
            int fileNameLength = BitConverter.ToInt32(data, 0);
            string fileName = System.Text.Encoding.UTF8.GetString(data, 4, fileNameLength);
            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            // 移除双引号、星号、小于号、大于号、问号、反斜杠、正斜杠、竖线、冒号和空格
            clientIP = clientIP.Replace("\"", "");
            clientIP = clientIP.Replace("*", "");
            clientIP = clientIP.Replace("<", "");
            clientIP = clientIP.Replace(">", "");
            clientIP = clientIP.Replace("?", "");
            clientIP = clientIP.Replace("\\", "");
            clientIP = clientIP.Replace("/", "");
            clientIP = clientIP.Replace("|", "");
            clientIP = clientIP.Replace(":", "");
            clientIP = clientIP.Replace(" ", "");
            BinaryWriter writer = new BinaryWriter(File.Open($"[{clientIP}]_" + fileName, FileMode.Append));
            writer.Write(data, 4 + fileNameLength, bytesRead - 4 - fileNameLength);

            // 继续接收并保存文件
            while ((bytesRead = stream.Read(data, 0, data.Length)) != 0)
            {
                writer.Write(data, 0, bytesRead);
            }

            // 关闭连接
            writer.Close();
            stream.Close();
            client.Close();
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // 关闭服务器
            server.Stop();
        }
    }
}