using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public LazyAdminWindow()
        {
            InitializeComponent();
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
}
