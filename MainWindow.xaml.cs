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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace LazyAdmin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string login = "abrakadabra";
        static string password = "abrakadabra";
        static string uninstallProgram;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnLogin(object sender, RoutedEventArgs e)
        {
            if(loginText.Text.Length > 0 && passwordText.Password.Length > 0)
            {
                login = loginText.Text;
                password = passwordText.Password;
                labelOpenFile.Content = "Your login: " + login;
                labelOpenInstallFile.Content = "Your password: " + password;
            }
        }
        private void btnLogout(object sender, RoutedEventArgs e)
        {

        }
        private void enterNameFileForUninstallBtn(object sender, RoutedEventArgs e)
        {
            if(lableUninstallSoft.Text.Length > 0)
            {
                uninstallProgram = lableUninstallSoft.Text;
            }
        }
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames) 
                { 
                    labelOpenFile.Content = System.IO.Path.GetFullPath(filename);
                }
            }
        }
        private void btnOpenInstallFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    labelOpenInstallFile.Content=System.IO.Path.GetFullPath(filename);
                }
            }
        }
    }
}
