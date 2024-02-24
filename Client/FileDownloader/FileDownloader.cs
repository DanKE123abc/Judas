using System;
using System.Net;
namespace JudasClient;

public class FileDownloader
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: FileDownloader <download_url> [download_file_name]");
            return;
        }

        string downloadUrl = args[0];
        string downloadFileName = args.Length > 1 ? args[1] : GetFileNameFromUrl(downloadUrl);

        using (WebClient client = new WebClient())
        {
            try
            {
                client.DownloadFile(downloadUrl, downloadFileName);
                Console.WriteLine($"File downloaded successfully: {downloadFileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }
        }
    }

    static string GetFileNameFromUrl(string url)
    {
        Uri uri = new Uri(url);
        return System.IO.Path.GetFileName(uri.LocalPath);
    }
}