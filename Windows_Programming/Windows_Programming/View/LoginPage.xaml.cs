﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Security.Cryptography;
using Windows.Storage;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Text;
using Windows_Programming.Database;
using System.Threading.Tasks;
using Windows_Programming.Helpers;
using Windows_Programming.Model;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private FirebaseServicesDAO firebaseServices = null;

        private ApplicationDataContainer localSettings;
        private ContentDialog loadingDialog;
        private string emailLocal = "";
        private string passwordLocal = "";
        private string emailDatabase = "";
        private string passwordDatabase = "";
        private string tokenLocal = "";
        private LoginWindow loginWindow;
        public LoginPage()
        {
            System.Diagnostics.Debug.WriteLine("7");
            this.InitializeComponent();
            System.Diagnostics.Debug.WriteLine("8");
            localSettings = ApplicationData.Current.LocalSettings;
            System.Diagnostics.Debug.WriteLine("9");

            //Get FirebaseService instance
            firebaseServices = FirebaseServicesDAO.Instance;

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
            if (tokenLocal!="")
            {
                var screen = new MainWindow();
                screen.Activate();
                // Dong LoginWindow da dong lan dau roi
                loginWindow.IsLoginWindowClosed = true;
                loginWindow.Close();



            }
        }
        //Catch Event enter
        private void InputLoginKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // Call the same login method as the button click
                LoginButtonClick(sender, new RoutedEventArgs());
            }
        }


        private async void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            string emailInput = EmailInputLogin_TextBox.Text;
            string passwordInput = PasswordInputLogin_TextBox.Password;



            if (emailInput == "" || passwordInput == "")
            {
                ShowDialog(emailInput == "" ? "Please enter email." : "Please enter password.");
                return;
            }
            if (CheckInput.CheckFormatEmail(emailInput) == false)
            {
                ShowDialog("Email is not valid.");
                return;
            }
            if (CheckInput.CheckFormatPassword(passwordInput) == false){
                ShowDialog("Password include at least 8 characters, 1 uppercase, 1 lowercase, 1 number, 1 special character: @ $ ! % * ? &");
                return;
            }
            await ReadFromDatabase(emailInput,passwordInput);
            //if (emailInput != emailDatabase || passwordInput != passwordDatabase)
            //{
            //    ShowDialog("Email or password is incorrect.");
            //    return;
            //}
            
        }

        private async  void ForgotPasswordClick(object sender, RoutedEventArgs e)
        {
            // Create the dialog
            TextBox emailInput = new TextBox
            {
                PlaceholderText = "Enter your email address",
                Margin = new Thickness(0, 10, 0, 0)
            };

            ContentDialog dialog = new ContentDialog
            {
                Title = "Reset Password",
                Content = emailInput,
                PrimaryButtonText = "Send Reset Link",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string email = emailInput.Text.Trim();

                if (string.IsNullOrEmpty(email))
                {
                    ShowDialog("Please enter an email address.");
                    return;
                }

                if (!CheckInput.CheckFormatEmail(email))
                {
                    ShowDialog("Please enter a valid email address.");
                    return;
                }

                CreateLoadingDialog();

                try
                {
                    var loadingTask = loadingDialog.ShowAsync();
                    await firebaseServices.SendPasswordResetEmailAsync(email);
                    loadingDialog.Hide();
                    ShowDialog("Password reset link has been sent to your email.");
                }
                catch (Exception ex)
                {
                    loadingDialog.Hide();
                    ShowDialog(ex.Message);
                }
            }

        }

        private void RegisterNowClick(object sender, RoutedEventArgs e)
        {
            // Navigate to RegisterPage
            Frame.Navigate(typeof(RegisterPage), loginWindow);
            Frame.BackStack.Clear();
        }
        //Read from database
        private async Task ReadFromDatabase(string emailInput, string passwordInput)
        {
            CreateLoadingDialog();

            // Show loading dialog
            
            try
            {
                var dialogTask = loadingDialog.ShowAsync();
                var userCredential = await firebaseServices.SignInWithEmailAndPasswordInFireBase(emailInput, passwordInput);

                // Get token and save to local settings
                string tokenResponse = await userCredential.User.GetIdTokenAsync();
                

                // Get account from Firestore
                var currentAccount = await firebaseServices.GetAccountByEmail(emailInput.Trim());
                WriteUserToLocal(currentAccount);
                if (currentAccount != null)
                {
                    if (RememberMeLogin_CheckBox.IsChecked == true)
                    {
                        WriteToLocal(tokenResponse);


                    }
                    else
                    {
                        DeleteFromLocal();
                    }

                    var screen = new MainWindow();
                    screen.Activate();
                    loginWindow?.Close();
                }
            }
            catch (Exception ex)
            {
                ShowDialog($"{ex.Message}");
                loadingDialog.Hide();
            }
            finally
            {
                // Hide loading dialog
                loadingDialog.Hide();
            }
        }

        private void CreateLoadingDialog()
        {
            StackPanel dialogContent = new StackPanel
            {
                Spacing = 10,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            ProgressRing progressRing = new ProgressRing
            {
                IsActive = true,
                Width = 50,
                Height = 50
            };

            TextBlock messageText = new TextBlock
            {
                Text = "Please wait...",
                HorizontalAlignment = HorizontalAlignment.Center
            };

            dialogContent.Children.Add(progressRing);
            dialogContent.Children.Add(messageText);

            loadingDialog = new ContentDialog
            {
                Content = dialogContent,
                IsPrimaryButtonEnabled = false,
                IsSecondaryButtonEnabled = false,
                XamlRoot = this.XamlRoot  // Set the XamlRoot from the current page
            };
        }
        //doc, luu, xoa db o local
        void ReadFromLocal()
        {

            if (localSettings.Values.ContainsKey("UserToken"))
            {
                
                tokenLocal = DecryptPassword(localSettings.Values["UserToken"] as string);
            }
            else
            {
                tokenLocal = "";
            }

        }

        void WriteToLocal(String userToken)
        {  

            if (localSettings.Values.ContainsKey("UserToken"))
            {
                return;
            }

            //chua luu
            string encryptedPasswordBase64 = EncryptPassword(userToken);// da luu entropy trong nay
            localSettings.Values["UserToken"] = encryptedPasswordBase64;
        }
        void WriteUserToLocal(Account account){
            localSettings.Values["Id"] = account.Id;
            localSettings.Values["Username"] = account.Username;
            localSettings.Values["Email"] = account.Email;
            localSettings.Values["Fullname"] = account.Fullname;
            localSettings.Values["Address"] = account.Address;
        }
        void DeleteFromLocal()
        {

            if (localSettings.Values.ContainsKey("UserToken"))
            {
                localSettings.Values.Remove("UserToken");
            }
        }

        //Hien dialog

        private void ShowDialog(string message)
        {
            NotificationLogin_TextBlock.Text = message;
            NotificationLogin_TextBlock.Opacity = 1;
            NotificationLogin_TextBlock.Visibility = Visibility.Visible;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, e) =>
            {
                NotificationLogin_TextBlock.Opacity = 0;
                NotificationLogin_TextBlock.Visibility = Visibility.Collapsed;
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