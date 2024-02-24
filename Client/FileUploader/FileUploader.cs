using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace JudasClient;

public class FileUploader
{
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: FileUploader <file_path> <hostname> <port>");
            return;
        }

        string filePath = args[0];
        string hostname = args[1];
        string port = args[2];

        // 连接服务器
        TcpClient client = new TcpClient(hostname, int.Parse(port));
        NetworkStream stream = client.GetStream();

        // 发送文件名和文件内容
        string fileName = Path.GetFileName(filePath);
        byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
        byte[] fileNameLength = BitConverter.GetBytes(fileNameBytes.Length);
        stream.Write(fileNameLength, 0, 4);
        stream.Write(fileNameBytes, 0, fileNameBytes.Length);

        using (FileStream fileStream = File.OpenRead(filePath))
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, bytesRead);
            }
        }

        // 关闭连接
        stream.Close();
        client.Close();
    }
}