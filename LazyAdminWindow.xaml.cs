using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            WOL.WakeOnLan(macAddress);
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
    }
    public static class WOL
    {

        public static async Task WakeOnLan(string macAddress)
        {
            byte[] magicPacket = BuildMagicPacket(macAddress);
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces().Where((n) =>
                n.NetworkInterfaceType != NetworkInterfaceType.Loopback && n.OperationalStatus == OperationalStatus.Up))
            {
                IPInterfaceProperties iPInterfaceProperties = networkInterface.GetIPProperties();
                foreach (MulticastIPAddressInformation multicastIPAddressInformation in iPInterfaceProperties.MulticastAddresses)
                {
                    IPAddress multicastIpAddress = multicastIPAddressInformation.Address;
                    if (multicastIpAddress.ToString().StartsWith("ff02::1%", StringComparison.OrdinalIgnoreCase)) // Ipv6: All hosts on LAN (with zone index)
                    {
                        UnicastIPAddressInformation unicastIPAddressInformation = iPInterfaceProperties.UnicastAddresses.Where((u) =>
                            u.Address.AddressFamily == AddressFamily.InterNetworkV6 && !u.Address.IsIPv6LinkLocal).FirstOrDefault();
                        if (unicastIPAddressInformation != null)
                        {
                            await SendWakeOnLan(unicastIPAddressInformation.Address, multicastIpAddress, magicPacket);
                            break;
                        }
                    }
                    else if (multicastIpAddress.ToString().Equals("224.0.0.1")) // Ipv4: All hosts on LAN
                    {
                        UnicastIPAddressInformation unicastIPAddressInformation = iPInterfaceProperties.UnicastAddresses.Where((u) =>
                            u.Address.AddressFamily == AddressFamily.InterNetwork && !iPInterfaceProperties.GetIPv4Properties().IsAutomaticPrivateAddressingActive).FirstOrDefault();
                        if (unicastIPAddressInformation != null)
                        {
                            await SendWakeOnLan(unicastIPAddressInformation.Address, multicastIpAddress, magicPacket);
                            break;
                        }
                    }
                }
            }
        }

        static byte[] BuildMagicPacket(string macAddress) // MacAddress in any standard HEX format
        {
            macAddress = Regex.Replace(macAddress, "[: -]", "");
            byte[] macBytes = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                macBytes[i] = Convert.ToByte(macAddress.Substring(i * 2, 2), 16);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < 6; i++)  //First 6 times 0xff
                    {
                        bw.Write((byte)0xff);
                    }
                    for (int i = 0; i < 16; i++) // then 16 times MacAddress
                    {
                        bw.Write(macBytes);
                    }
                }
                return ms.ToArray(); // 102 bytes magic packet
            }
        }

        static async Task SendWakeOnLan(IPAddress localIpAddress, IPAddress multicastIpAddress, byte[] magicPacket)
        {
            using (UdpClient client = new UdpClient(new IPEndPoint(localIpAddress, 0)))
            {
                await client.SendAsync(magicPacket, magicPacket.Length, multicastIpAddress.ToString(), 9);
            }
        }
    }
}