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
using System.Diagnostics;

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
        private void btn_ClickInstallSoft(object sender, RoutedEventArgs e)
        {
            var proccess = new Process();
            proccess.StartInfo.FileName = "powershell.exe";
            proccess.StartInfo.Arguments = @"-executionpolicy unrestricted D:\install.ps1";
            proccess.Start();
            proccess.WaitForExit();
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
/**
 function RunInstall ($login, $password, $pathClass, $pathProg){ 
    $securePassword = ConvertTo-SecureString $password -AsPlainText -force 
    $credential = New-Object System.Management.Automation.PsCredential($login,$securePassword)
    [string[]]$ar = Get-Content -Path $pathClass
    $session = New-PSSession -computername $ar -credential $credential
    $pathInst = $pathProg -replace "E:\Install\" , ""
    $endPath = 'D:\' + $pathInst
    Copy-Item -Path $pathProg -ToSession $session -Destination 'D:\' -Recurse -Force
    Invoke-Command $session -ScriptBlock {Invoke-CimMethod -ClassName Win32_Product -MethodName Install -Arguments @{PackageLocation=$endPath}}
    Invoke-Command $session -ScriptBlock {Remove-Item $endPath -force}
    Remove-PSSession $session
}
write-host "Function RunInstall created. RunInstall login password pathClass pathProg"

function WOL ($pathClass) {
    function Send-WOL
    {
        [CmdletBinding()]
        param(
        [Parameter(Mandatory=$True,Position=1)]
        [string]$mac,
        [string]$ip="255.255.255.0", 
        [int]$port=9
        )
        $broadcast = [Net.IPAddress]::Parse($ip)
 
        $mac=(($mac.replace(":","")).replace("-","")).replace(".","")
        $target=0,2,4,6,8,10 | % {[convert]::ToByte($mac.substring($_,2),16)}
        $packet = (,[byte]255 * 6) + ($target * 16)
 
        $UDPclient = new-Object System.Net.Sockets.UdpClient
        $UDPclient.Connect($broadcast,$port)
        [void]$UDPclient.Send($packet, 102) 
    }

    [string[]]$ar = Get-Content -Path $pathClass
    for ($i = 0; $i -lt $ar.Count; $i = $i +2)
    {
        $j = $i + 1
        Send-WOL -mac $ar[$i] -ip $ar[$j]
    }
}
write-host "Function WOL created. WOL pathClass"

function Uninstall ($login, $password, $pathClass, $prog) {
    $securePassword = ConvertTo-SecureString $password -AsPlainText -force 
    $credential = New-Object System.Management.Automation.PsCredential($login,$securePassword)
    [string[]]$ar = Get-Content -Path $pathClass
    $session = New-PSSession -computername $ar -credential $credential
    Invoke-Command $session {Get-CimInstance -Class Win32_Product -Filter "Name=$prog" | Invoke-CimMethod -MethodName Uninstall}
    Remove-PSSession $session
}
write-host "Function Uninstall created. Uninstall login password pathClass prog"

function OffLine ($login, $password, $pathClass) {
    $securePassword = ConvertTo-SecureString $password -AsPlainText -force 
    $credential = New-Object System.Management.Automation.PsCredential($login,$securePassword)
    [string[]]$ar = Get-Content -Path $pathClass
    $session = New-PSSession -computername $ar -credential $credential
    Invoke-Command $session {shutdown /s /t 0}
    Remove-PSSession $session
}
write-host "Function OffLine created. OffLine login password pathClass"

function Check ($pathClass) {
    function Check-Online {
    param($pc)
    test-connection -count 1 -ComputerName $pc -TimeToLive 5 -asJob |
    Wait-Job |
    Receive-Job |
    Where-Object { $_.StatusCode -eq 0 } |
    Select-Object -ExpandProperty Address
    }

    [string[]]$ar = Get-Content -Path $pathClass
    $pcoff = @()
    $pcall = @()
    for ($i = 1; $i -lt $ar.Count; $i = $i +2)
    {
        $pcall += $ar[$i]
        $online = Check-Online -pc $ar[$i]
        $pcoff += $online
    }

    write-host "PC On"
    write-host ""
    $pcoff
    write-host ""
    write-host "PC Off"
    Compare-Object $pcall $pcoff
}
write-host "Function Check created. Check pathClass"
 
 */
