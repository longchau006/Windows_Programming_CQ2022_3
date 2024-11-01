using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Security.Cryptography;
using Windows.Storage;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Text;
using Windows_Programming.Database;
using System.Threading.Tasks;
using Firebase.Auth;
using Windows_Programming.Model;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {   
        private FirebaseServicesDAO firebaseServices;

        private ApplicationDataContainer localSettings;
        private LoginWindow loginWindow;
        String tokenLocal = "";

        private ContentDialog loadingDialog;
        public RegisterPage()
        {
            this.InitializeComponent();
            localSettings = ApplicationData.Current.LocalSettings;

            //Get FirebaseService instance
            firebaseServices = FirebaseServicesDAO.Instance;

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
        private async void RegisterButtonClick(object sender, RoutedEventArgs e)
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


            await WriteToDatabase(emailInput, passwordInput);
            
        }

        private void LoginNowClick(object sender, RoutedEventArgs e)
        {
            // Navigate to RegisterPage
            Frame.Navigate(typeof(LoginPage), loginWindow);
            Frame.BackStack.Clear();
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
        void DeleteFromLocal()
        {

            if (localSettings.Values.ContainsKey("UserToken"))
            {
                localSettings.Values.Remove("UserToken");
            }
        }

        //Write to Database
        //async Task WriteToDatabase(string emailInput, string passwordInput)
        //{
        //    try
        //    {
        //        var userCredential = await firebaseServices.CreateAccountInFireBase(
        //            emailInput,
        //            passwordInput
        //        );

        //        // Get user info
        //        var user = userCredential.User;
        //        var email = user.Info.Email;
        //        var uid = user.Uid;

        //        // Create user in Firestore
        //        int numberCurrentAccounts=await firebaseServices.GetAccountsCount();

        //        //Add user to firestore
        //        var newAccount = new Account 
        //        {
        //            Id = numberCurrentAccounts,
        //            Username = email,
        //            Email = email,
        //            Fullname = email,
        //            Address = ""
        //        };
        //        await firebaseServices.CreateAccountInFirestore(newAccount);

        //        //var getAccount = await firebaseServices.GetAccountByID(newAccount.Id);

        //        //getAccount.PrintAccountInfo();

        //        // Show success message
        //        System.Diagnostics.Debug.WriteLine("Create ok");
        //    }
        //    catch (FirebaseAuthException ex)
        //    {
        //        // Show error message
        //        System.Diagnostics.Debug.WriteLine("Create sai");
        //    }
        //}
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

        private async Task WriteToDatabase(string emailInput, string passwordInput)
        {
            CreateLoadingDialog();
            var dialogTask = loadingDialog.ShowAsync(); // Store the task

            try
            {
                var userCredential = await firebaseServices.CreateAccountInFireBase(emailInput, passwordInput);

                var user = userCredential.User;
                tokenLocal = await user.GetIdTokenAsync();

                if (RememberMeRegister_CheckBox.IsChecked == true)
                {
                    WriteToLocal(tokenLocal);
                }
                else
                {
                    DeleteFromLocal();
                }

                var screen = new MainWindow();
                screen.Activate();
                loginWindow?.Close();

                int numberCurrentAccounts = await firebaseServices.GetAccountsCount();

                var newAccount = new Account
                {
                    Id = numberCurrentAccounts,
                    Username = emailInput,
                    Email = emailInput,
                    Fullname = emailInput,
                    Address = ""
                };

                await firebaseServices.CreateAccountInFirestore(newAccount);

                // Close the dialog by completing the task
                await dialogTask;
            }
            catch (Exception ex)
            {
                ShowDialog(ex.Message);
                // Close the dialog in case of error
                await dialogTask;
            }
        }


        //Hien dialog

        private void ShowDialog(string message)
        {
            NotificationRegister_TextBlock.Text = message;
            NotificationRegister_TextBlock.Opacity = 1;
            NotificationRegister_TextBlock.Visibility = Visibility.Visible;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, e) =>
            {
                NotificationRegister_TextBlock.Opacity = 0;
                NotificationRegister_TextBlock.Visibility = Visibility.Collapsed;
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