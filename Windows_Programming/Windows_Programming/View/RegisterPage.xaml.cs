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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        private ApplicationDataContainer localSettings;
        private LoginWindow loginWindow;
        public RegisterPage()
        {
            this.InitializeComponent();
            localSettings = ApplicationData.Current.LocalSettings;

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            loginWindow = e.Parameter as LoginWindow;
            System.Diagnostics.Debug.WriteLine("Khoi tao 2");
            if (loginWindow != null)
            {
                loginWindow.Title = "Register";
            }
        }
        private void RegisterButtonClick(object sender, RoutedEventArgs e)
        {
            string emailInput = EmailInputRegister_TextBox.Text;
            string passwordInput = PasswordInputRegister_TextBox.Password;
            string confirmPasswordInput = ConfirmPasswordInputRegister_TextBox.Password;



            if (emailInput == "" || passwordInput == "" || confirmPasswordInput == "")
            {
                ShowDialog(emailInput == "" ? "Please enter email." : (passwordInput == "" ? "Please enter password." : "Please enter confirm password."));
                return;
            }

            if (!CheckFormatEmail(emailInput))
            {
                ShowDialog("Invalid email format");
                return;
            }
            if (!CheckUsedEmail(emailInput))
            {
                ShowDialog("Email has been used");
                return;
            }
            if (!CheckFormatPassword(passwordInput))
            {
                ShowDialog("Password must contain at least 8 characters, including UPPER/lowercase and numbers");
                return;
            }
            if (!CheckPasswordAndConfirmPassword(passwordInput, confirmPasswordInput))
            {
                ShowDialog("Password and Confirm Password are not the same");
                return;
            }
            if (RememberMeRegister_CheckBox.IsChecked == true)
            {
                WriteToLocal(emailInput, passwordInput);
            }
            else
            {

                DeleteFromLocal();
            }

            WriteToDatabase(emailInput, passwordInput);

            var screen = new MainWindow();
            screen.Activate();
            // Dong loginWindow

            loginWindow?.Close();
        }

        private void LoginNowClick(object sender, RoutedEventArgs e)
        {
            // Navigate to RegisterPage
            Frame.Navigate(typeof(LoginPage), loginWindow);
            Frame.BackStack.Clear();
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

        //Write to Database
        void WriteToDatabase(string emailInput, string passwordInput)
        {
            //Write to database
        }

        //Hien dialog

        private void ShowDialog(string message)
        {
            NotificationRegister_TextBlock.Text = message;
            NotificationRegister_TextBlock.Opacity = 1;


            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, e) =>
            {
                NotificationRegister_TextBlock.Opacity = 0;
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
        // May ham ho tro
        private bool CheckFormatEmail(string email)
        {
            return true;
        }

        private bool CheckUsedEmail(string email)
        {
            return true;
        }

        private bool CheckFormatPassword(string password)
        {
            return true;
        }

        private bool CheckPasswordAndConfirmPassword(string password, string confirmPassword)
        {
            return true;
        }
    }
}