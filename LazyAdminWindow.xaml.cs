using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace LazyAdmin
{
    /// <summary>
    /// Логика взаимодействия для LazyAdminWindow.xaml
    /// </summary>
    public partial class LazyAdminWindow : Window
    {
        string login, password;
        string macAddress = "38D547B5206A";
        string[] ips
        {
            get { return this.ips; }
            set
            {
                ips = value;
                if (ips == null)
                {
                    PowerOffBtn.IsEnabled = false;
                }
            }
        }

        private void TurnOfPCByIp(string[] ip)
        {
            foreach (string s in ip)
            {
                ProcessStartInfo shutdown = new ProcessStartInfo("shutdown", @"-m \\" + s + "-s -f -t 0");
                shutdown.CreateNoWindow = true;
                shutdown.UseShellExecute = false;
                Process.Start(shutdown);
            }
        }
        public LazyAdminWindow()
        {
            InitializeComponent();
            // WOL.WakeOnLan(macAddress);
            InitializeGetIPsAndMac();
            
            foreach(PCInfo pCInfo in list)
            {
                string hostName = "Uknown";
                try
                {
                    hostName = Dns.GetHostEntry(pCInfo.IP).HostName;
                }catch(Exception e) { }
                pCInfo.HOSTNAME = hostName;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Name == "fileNameTB")
            {
                if (textBox.Text == "ENTER FILE NAME HERE")
                {
                    textBox.Text = "";
                }
            }
            else
            {
                if (textBox.Text == "ENTER LOGIN HERE")
                {
                    textBox.Text = "";
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Name == "fileNameTB")
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                    textBox.Text = "ENTER FILE NAME HERE";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                    textBox.Text = "ENTER LOGIN HERE";
            }
        }

        private void ClickSelectFile(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if ("Select txt file" == btn.Content.ToString())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
            }
            else if ("Select exe file" == btn.Content.ToString())
            {
                openFileDialog.Filter = "Executable files (*.exe)|*.exe";
            }
            if (openFileDialog.ShowDialog() == true)
            {
                if ("Select txt file" == btn.Content.ToString())
                {
                    ipsLbl.Content = openFileDialog.FileName;
                }
                else if ("Select exe file" == btn.Content.ToString())
                {
                    instalFileLbl.Content = openFileDialog.FileName;
                }
            }
        }


        private static List<PCInfo> list;

        private static StreamReader ExecuteCommandLine(String file, String arguments = "")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = file;
            startInfo.Arguments = arguments;

            Process process = Process.Start(startInfo);

            return process.StandardOutput;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1();
            window.listView.ItemsSource = list;
            window.ShowDialog();
        }

        private static void InitializeGetIPsAndMac()
        {
            if (list != null)
                return;

            var arpStream = ExecuteCommandLine("arp", "-a");
            List<string> result = new List<string>();
            while (!arpStream.EndOfStream)
            {
                var line = arpStream.ReadLine().Trim();
                result.Add(line);
            }

            list = result.Where(x => !string.IsNullOrEmpty(x) && (x.Contains("dynamic") || x.Contains("static")))
                .Select(x =>
                {
                    string[] parts = x.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    return new PCInfo { IP = parts[0].Trim(), MAC = parts[1].Trim() };
                }).ToList();
        }
        void CopyFile()
        {
            System.IO.File.Copy("sourcePath", "\\machinename\\DriveLetter$\folder name");
        }
    }

    class PCInfo
    {
        public string IP { get; set; }
        public string MAC { get; set; }
        public string HOSTNAME { get; set; }
    }
}