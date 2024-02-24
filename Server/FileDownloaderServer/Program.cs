using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
namespace JudasServer;

class Program
{

    static async Task Main(string[] args)
    {
        if (!IsAdministrator())
        {
            Console.WriteLine("You must run this program as an administrator.");
            RestartAsAdmin(args);
            return;
        }

        if (args.Length < 2)
        {
            Console.WriteLine("Usage: FileDownloaderServer <file_path> <port>");
            return;
        }

        string filePath = args[0];
        string port = args[1];

        // 创建HTTP服务器
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://127.0.0.1:{port}/"); // 监听所有IPv4地址
        listener.Prefixes.Add($"http://[::1]:{port}/"); // 监听所有IPv6地址
        listener.Start();
        Console.WriteLine("Server is running...");

        // 打印对外的IPv4和IPv6地址
        string ipv4Address = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
        string ipv6Address = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
        Console.WriteLine($"IPv4 Address: http://{ipv4Address}:{port}");
        Console.WriteLine($"IPv6 Address: http://[{ipv6Address}]:{port}");

        // 挂载文件
        while (true)
        {
            var context = await listener.GetContextAsync();
            var response = context.Response;

            using (var fileStream = File.OpenRead(filePath))
            {
                response.ContentLength64 = fileStream.Length;
                response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                response.StatusCode = 200;
                await fileStream.CopyToAsync(response.OutputStream);
            }

            response.Close();
        }
    }

    public static bool IsAdministrator()
    {
        bool result;
        try
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            result = principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            result = false;
        }

        return result;
    }

    // 以管理员身份重新启动应用程序
    static void RestartAsAdmin(string[] args)
    {
        var startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = true;
        startInfo.WorkingDirectory = Environment.CurrentDirectory;
        startInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
        startInfo.Arguments = string.Join(" ", args);
        startInfo.Verb = "runas"; // 请求管理员权限
        try
        {
            Process.Start(startInfo);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // 用户取消了UAC提示
        }

        Environment.Exit(0);
    }


}