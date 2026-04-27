using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sever
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            MessageBox.Show("Server Online");

            Task.Run(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = server.AcceptTcpClient();
                        byte[] buffer = new byte[2048];
                        int bytes = client.GetStream().Read(buffer, 0, buffer.Length);
                        string data = Encoding.UTF8.GetString(buffer, 0, bytes);

                        this.Invoke(new Action(() => {
                            string[] parts = data.Split('|');
                            if (parts.Length == 4)
                            {
                                ListViewItem item = new ListViewItem(DateTime.Now.ToString("HH:mm:ss")); // Cột 1: Thời gian

                                item.SubItems.Add(parts[0]); // Cột 2: Máy trạm
                                item.SubItems.Add(parts[2]); // Cột 3: Hành động tạo/xóa/sửa 
                                item.SubItems.Add(parts[3]); // Cột 4: Đường dẫn 
                                item.SubItems.Add(parts[1]); // Cột 5: IP

                                listView1.Items.Insert(0, item);

                                // Lưu trữ lịch sử vào file CSV
                                System.IO.File.AppendAllLines("logs.csv", new[] { $"{DateTime.Now},{parts[0]},{parts[1]},{parts[2]},{parts[3]}" });

                                // Phát ra âm thanh thông báo khi có sự thay đổi
                                System.Media.SystemSounds.Beep.Play();
                            }
                        }));
                        client.Close();
                    }
                    catch { }
                }
            });
        }
    }
}