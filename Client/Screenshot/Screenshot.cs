using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JudasClient;

public class Screenshot
{
    public static void Main()
    {
        Start();
    }
    
    public static void Start(int width = default , int height = default , string file = "screenshot.png")
    {
        IntPtr screenDC = GetDC(IntPtr.Zero);
        int screenWidth;
        int screenHeight;
        if (width == default && height == default)
        {
            screenWidth = GetDeviceCaps(screenDC, (int)DeviceCap.HORZRES);
            screenHeight = GetDeviceCaps(screenDC, (int)DeviceCap.VERTRES);
        }
        else
        {
            screenWidth = width;
            screenHeight = height;
        }
        string filePath = file;
        
        try
        {
            Image image = new Bitmap(screenWidth, screenHeight);
            Graphics g = Graphics.FromImage(image);
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0),new Size(screenWidth, screenHeight));
            image.Save(filePath, ImageFormat.Png);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            IntPtr memoryDC = CreateCompatibleDC(screenDC);
            IntPtr bitmap = CreateCompatibleBitmap(screenDC, screenWidth, screenHeight);
            IntPtr oldBitmap = SelectObject(memoryDC, bitmap);
            BitBlt(memoryDC, 0, 0, screenWidth, screenHeight, screenDC, 0, 0, CopyPixelOperation.SourceCopy);
            Bitmap resizedBitmap = new Bitmap(1920, 1080);
            using (Graphics g = Graphics.FromImage(resizedBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(Image.FromHbitmap(bitmap), 0, 0, 1920, 1080);
            }
            resizedBitmap.Save(filePath, ImageFormat.Png);
            SelectObject(memoryDC, oldBitmap);
            DeleteObject(bitmap);
            DeleteDC(memoryDC);
            ReleaseDC(IntPtr.Zero, screenDC);
        }
        

        Console.WriteLine($"Screenshot saved to: {filePath}");
        
        
        
    }

    #region Windows API 函数
    
    // Windows API 声明

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern int GetDeviceCaps(IntPtr hdc, int index);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

    [DllImport("gdi32.dll")]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

    [DllImport("gdi32.dll")]
    public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int width, int height, IntPtr hdcSrc, int xSrc, int ySrc, CopyPixelOperation rop);

    [DllImport("gdi32.dll")]
    public static extern int DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern int DeleteObject(IntPtr hObject);

    // 设备能力常量

    public enum DeviceCap
    {
        HORZRES = 8,
        VERTRES = 10
    }

    // 像素操作常量

    public enum CopyPixelOperation
    {
        SourceCopy = 0x00CC0020
    }

    // 自定义函数，用于保存位图为文件

    public static void SaveBitmapToFile(IntPtr hBitmap, string filePath)
    {
        using (Bitmap bitmap = Image.FromHbitmap(hBitmap))
        {
            bitmap.Save(filePath, ImageFormat.Png);
        }
    }
    
    #endregion
    
    
    
}
