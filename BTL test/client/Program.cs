using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


// ĐỊNH NGHĨA THƯ MỤC CẦN GIÁM SÁT
string folderToWatch = @"C:\Users\buitr\source\repos\test";
Console.OutputEncoding = System.Text.Encoding.UTF8;

if (!Directory.Exists(folderToWatch))
{
    Console.WriteLine("LOI: Thu muc khong ton tai");
    return;
}

// THIẾT LẬP WATCHER
FileSystemWatcher watcher = new FileSystemWatcher(folderToWatch);

// Giám sát các thay đổi về file
watcher.NotifyFilter = NotifyFilters.FileName
                     | NotifyFilters.LastWrite
                     | NotifyFilters.DirectoryName;

// Đăng ký các sự kiện và gửi dữ liệu về Server
watcher.Created += (s, e) => SendToServer("Created", e.FullPath);
watcher.Deleted += (s, e) => SendToServer("Deleted", e.FullPath);
watcher.Changed += (s, e) => SendToServer("Changed", e.FullPath);
watcher.Renamed += (s, e) => SendToServer("Renamed", $"{e.OldName} -> {e.Name}");

// Bắt đầu giám sát
watcher.EnableRaisingEvents = true;

Console.Clear();
Console.WriteLine("==================================================");
Console.WriteLine("   HE THONG GIAM SAT FILE - PHAN MEM MAY TRAM     ");
Console.WriteLine("==================================================");
Console.WriteLine($"Dang theo doi tai: {folderToWatch}");
Console.WriteLine("Trang thai: Dang ket noi voi Server (Port 5000)");
Console.WriteLine("==================================================");
Console.WriteLine("Nhan 'q' de dung chuong trinh ");
Console.WriteLine();

// Giữ chương trình chạy cho đến khi nhấn 'q'
while (Console.ReadKey().KeyChar != 'q') ;

// HÀM GỬI DỮ LIỆU QUA SOCKET
void SendToServer(string action, string filePath)
{
    try
    {
        // Lấy tên máy tính hiện tại
        string hostName = Dns.GetHostName();

        // Đóng gói gói tin theo định dạng: TenMay|IP|HanhDong|DuongDan
        string dataPacket = $"{hostName}|127.0.0.1|{action}|{filePath}";

        // Kết nối đến Server tại localhost port 5000
        using TcpClient client = new TcpClient("127.0.0.1", 5000);
        byte[] data = Encoding.UTF8.GetBytes(dataPacket);
        client.GetStream().Write(data, 0, data.Length);

        // HIỆN THÔNG BÁO LÊN MÀN HÌNH CLIENT
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] -> PHAT HIEN: {action}");
        Console.WriteLine($"             Doi tuong: {Path.GetFileName(filePath)}");
        Console.WriteLine($"             Trang thai: Da bao cao ve Server thanh cong");
        Console.ResetColor();
        Console.WriteLine("--------------------------------------------------");
    }
    catch
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] LOI: Khong the ket noi den Server!");
        Console.ResetColor();
    }
}