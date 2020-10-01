using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
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
        string macAddress = "00D861D2ABD1";
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
            WakeFunction(macAddress);
        }
        private void WakeFunction(string MAC_ADDRESS)
        {
            WOLClass client = new WOLClass();
            client.Connect(new
               IPAddress(0xffffffff),  //255.255.255.255  i.e broadcast
               0x9); // port=12287 let's use this one 
            client.SetClientToBrodcastMode();
            //set sending bites
            int counter = 0;
            //buffer to be send
            byte[] bytes = new byte[1024];   // more than enough :-)
                                             //first 6 bytes should be 0xFF
            for (int y = 0; y < 6; y++)
                bytes[counter++] = 0xFF;
            //now repeate MAC 16 times
            for (int y = 0; y < 16; y++)
            {
                int i = 0;
                for (int z = 0; z < 6; z++)
                {
                    bytes[counter++] =
                        byte.Parse(MAC_ADDRESS.Substring(i, 2),
                        NumberStyles.HexNumber);
                    i += 2;
                }
            }

            //now send wake up packet
            int reterned_value = client.Send(bytes, 1024);
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
            if("Select txt file" == btn.Content.ToString())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
            }
            else if("Select exe file" == btn.Content.ToString())
            {
                openFileDialog.Filter = "Executable files (*.exe)|*.exe";
            }
            if(openFileDialog.ShowDialog() == true)
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
    public class WOLClass : UdpClient
    {
        public WOLClass() : base()
        { }
        //this is needed to send broadcast packet
        public void SetClientToBrodcastMode()
        {
            if (this.Active)
                this.Client.SetSocketOption(SocketOptionLevel.Socket,
                                          SocketOptionName.Broadcast, false);
        }
    }
}
