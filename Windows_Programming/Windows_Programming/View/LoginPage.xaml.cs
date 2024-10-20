using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Security.Cryptography;
using Windows.Storage;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Text;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private ApplicationDataContainer localSettings;
        private string emailLocal = "";
        private string passwordLocal = "";
        private string emailDatabase = "";
        private string passwordDatabase = "";
        private LoginWindow loginWindow;
        public LoginPage()
        {
            System.Diagnostics.Debug.WriteLine("7");
            this.InitializeComponent();
            System.Diagnostics.Debug.WriteLine("8");
            localSettings = ApplicationData.Current.LocalSettings;
            System.Diagnostics.Debug.WriteLine("9");

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("10");
            base.OnNavigatedTo(e);
            loginWindow = e.Parameter as LoginWindow;
            //Phai them cho nay vi naviagteTo chay sau ham constructor
            PreProcess();
            System.Diagnostics.Debug.WriteLine("11");
        }
        //Tien xu li load infor
        void PreProcess()
        {
            if (loginWindow != null)
            {
                loginWindow.Title = "Login";
            }
            ReadFromLocal();
            //showDialog($"{usernameLocal} va {passwordLocal}");
            //trc do da luu roi th� do thang loginWindow 
            if (emailLocal != "" && passwordLocal != "")
            {
                var screen = new MainWindow();
                screen.Activate();
                // Dong LoginWindow da dong lan dau roi
                loginWindow.IsLoginWindowClosed = true;
                loginWindow.Close();



            }
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            string emailInput = EmailInputLogin_TextBox.Text;
            string passwordInput = PasswordInputLogin_TextBox.Password;



            if (emailInput == "" || passwordInput == "")
            {
                ShowDialog(emailInput == "" ? "Please enter email." : "Please enter password.");
                return;
            }

            ReadFromDatabase();
            if (emailInput != emailDatabase || passwordInput != passwordDatabase)
            {
                ShowDialog("Email or password is incorrect.");
                return;
            }
            if (RememberMeLogin_CheckBox.IsChecked == true)
            {

                WriteToLocal(emailInput, passwordInput);
            }
            else
            {

                DeleteFromLocal();
            }

            var screen = new MainWindow();
            screen.Activate();

            loginWindow?.Close();

        }

        private void ForgotPasswordClick(object sender, RoutedEventArgs e)
        {

        }

        private void RegisterNowClick(object sender, RoutedEventArgs e)
        {
            // Navigate to RegisterPage
            Frame.Navigate(typeof(RegisterPage), loginWindow);
            Frame.BackStack.Clear();
        }
        //Read from database
        void ReadFromDatabase()
        {
            emailDatabase = "admin";
            passwordDatabase = "admin";
        }

        //doc, luu, xoa db o local
        void ReadFromLocal()
        {
            if (localSettings.Values.ContainsKey("email") && localSettings.Values.ContainsKey("passwordInBase64"))
            {
                emailLocal = localSettings.Values["email"] as string;
                passwordLocal = DecryptPassword(localSettings.Values["passwordInBase64"] as string);
            }
            else
            {
                emailLocal = "";
                passwordLocal = "";
            }
        }

        void WriteToLocal(String emailInput, String passwordInput)
        {   //Neu co usernam roi, thi khong luu nua
            if (localSettings.Values.ContainsKey("email"))
            {
                return;
            }

            //chua luu
            string encryptedPasswordBase64 = EncryptPassword(passwordInput);// da luu entropy trong nay
            localSettings.Values["email"] = emailInput;
            localSettings.Values["passwordInBase64"] = encryptedPasswordBase64;
        }
        void DeleteFromLocal()
        {

            if (localSettings.Values.ContainsKey("email"))
            {
                localSettings.Values.Remove("email");
                localSettings.Values.Remove("passwordInBase64");
                localSettings.Values.Remove("entropyInBase64");
            }
        }

        //Hien dialog

        private void ShowDialog(string message)
        {
            NotificationLogin_TextBlock.Text = message;
            NotificationLogin_TextBlock.Opacity = 1;


            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, e) =>
            {
                NotificationLogin_TextBlock.Opacity = 0;
                timer.Stop(); // 
            };
            timer.Start(); //
        }



        /// ma hoa va giai ma hoa
        private string EncryptPassword(string passwordInput)
        {
            var passwordInBytes = Encoding.UTF8.GetBytes(passwordInput);
            var entropyInBytes = new byte[20];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(entropyInBytes);
            }

            var encryptedPasswordInBytes = ProtectedData.Protect(
                passwordInBytes,
                entropyInBytes,
                DataProtectionScope.CurrentUser);

            var encryptedPasswordBase64 = Convert.ToBase64String(encryptedPasswordInBytes);
            var entropyInBase64 = Convert.ToBase64String(entropyInBytes);

            localSettings.Values["entropyInBase64"] = entropyInBase64;

            return encryptedPasswordBase64;
        }

        private string DecryptPassword(string passwordInBase64)
        {
            var encryptedPasswordInBytes = Convert.FromBase64String(passwordInBase64);

            var entropyInBase64 = localSettings.Values["entropyInBase64"] as string;
            var entropyInBytes = Convert.FromBase64String(entropyInBase64);

            var passwordInBytes = ProtectedData.Unprotect(
                encryptedPasswordInBytes,
                entropyInBytes,
                DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(passwordInBytes);
        }

    }
}