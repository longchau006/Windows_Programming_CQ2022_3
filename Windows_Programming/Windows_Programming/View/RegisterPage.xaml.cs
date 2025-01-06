using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Security.Cryptography;
using Windows.Storage;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Text;
using Windows_Programming.Database;
using System.Threading.Tasks;
using Windows_Programming.Model;
using Windows_Programming.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {   
        private FirebaseServicesDAO firebaseServices = null;

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

        //Catch Signup button click enter
        private void InputRegisterKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // Call the same login method as the button click
                RegisterButtonClick(sender, new RoutedEventArgs());
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

            if (CheckInput.CheckFormatEmail(emailInput) == false)
            {
                ShowDialog("Email is not valid.");
                return;
            }
            if (CheckInput.CheckFormatPassword(passwordInput) == false){
                ShowDialog("Password include at least 8 characters, 1 uppercase, 1 lowercase, 1 number, 1 special character: @ $ ! % * ? &");
                return;
            }
            if (passwordInput!=confirmPasswordInput)
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
            //if (localSettings.Values.ContainsKey("Username"))
            //{
            //    localSettings.Values.Remove("Username");
            //}
            //if (localSettings.Values.ContainsKey("Email"))
            //{
            //    localSettings.Values.Remove("Email");
            //}
            //if (localSettings.Values.ContainsKey("Fullname"))
            //{
            //    localSettings.Values.Remove("Fullname");
            //}
            //if (localSettings.Values.ContainsKey("Address"))
            //{
            //    localSettings.Values.Remove("Address");
            //}
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
            if (loadingDialog != null)
            {
                loadingDialog.Hide();
                loadingDialog = null;
            }

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
                XamlRoot = this.XamlRoot
            };
        }
        private async Task WriteToDatabase(string emailInput, string passwordInput)
        {
            CreateLoadingDialog();
            var dialogTask = loadingDialog.ShowAsync();

            try
            {
                var userCredential = await firebaseServices.CreateAccountInFireBase(emailInput, passwordInput);
                if (userCredential != null)
                {
                    var user = userCredential.User;
                    tokenLocal = await user.GetIdTokenAsync();


                    int numberCurrentAccounts = await firebaseServices.GetMaxAccountId();
                    var newAccount = new Account
                    {
                        Id = numberCurrentAccounts+1,
                        Username = emailInput,
                        Email = emailInput,
                        Fullname = emailInput,
                        Address = ""
                    };
                    WriteUserToLocal(newAccount);

                    await firebaseServices.CreateAccountInFirestore(newAccount);

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
                }

                // Complete the dialog task
                await dialogTask;
            }
            catch (Exception ex)
            {
                if (loadingDialog != null)
                {
                    loadingDialog.Hide();
                }
                ShowDialog(ex.Message);
            }
            finally
            {
                if (loadingDialog != null)
                {
                    loadingDialog.Hide();
                }
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
    }
}