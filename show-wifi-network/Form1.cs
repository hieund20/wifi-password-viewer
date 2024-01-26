using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace show_wifi_network
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Thiết lập thuộc tính của ListView
            listView1.View = View.Details;
            listView1.FullRowSelect = true;

            // Thêm cột cho tên mạng Wi-Fi
            listView1.Columns.Add("Tên mạng Wi-Fi đã kết nối", 500); // 200 là chiều rộng của cột, bạn có thể điều chỉnh

            // Gắn sự kiện cho ListView
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
        }

        private void show_btn_Click(object sender, EventArgs e)
        {
            string command = "netsh wlan show profiles";

            // Tạo một ProcessStartInfo để cấu hình quá trình chạy
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe"; // Sử dụng cmd.exe trên Windows
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            // Tạo một quá trình mới
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            // Gửi lệnh vào đầu vào tiêu chuẩn của quá trình
            process.StandardInput.WriteLine(command);
            process.StandardInput.Flush();
            process.StandardInput.Close();

            // Đọc đầu ra từ quá trình
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(); // Chờ quá trình kết thúc

            // Phân tách output thành các dòng và thêm vào ListView
            string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                // Chỉ thêm các dòng chứa thông tin về các mạng Wi-Fi đã kết nối
                if (line.StartsWith("    All User Profile"))
                {
                    // Lấy tên mạng Wi-Fi từ dòng
                    string wifiName = line.Substring("    All User Profile     : ".Length).Trim();

                    // Thêm tên mạng Wi-Fi vào ListView
                    listView1.Items.Add(wifiName);
                }
            }

            // Đóng quá trình sau khi sử dụng
            process.Close(); 
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn không
            if (listView1.SelectedItems.Count > 0)
            {
                // Lấy tên mạng Wi-Fi từ mục được chọn
                string selectedWiFi = listView1.SelectedItems[0].Text;

                // Thực hiện lệnh netsh để hiển thị mật khẩu
                string command = $"netsh wlan show profile name=\"{selectedWiFi}\" key=clear";

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();

                process.StandardInput.WriteLine(command);
                process.StandardInput.Flush();
                process.StandardInput.Close();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Lọc ra Key Content (nội dung của mật khẩu) từ output
                int startIndex = output.IndexOf("Key Content") + "Key Content".Length;
                int endIndex = output.IndexOf("Cost settings");
                string keyContent = output.Substring(startIndex, endIndex - startIndex).Trim();

                // Hiển thị output trong một cửa sổ mới hoặc control khác tùy thuộc vào yêu cầu của bạn
                MessageBox.Show($"Mật khẩu Wi-Fi là {keyContent}", "Mật khẩu Wi-Fi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Đóng quá trình sau khi sử dụng                
                process.Close();
            }
        }
    }
}
