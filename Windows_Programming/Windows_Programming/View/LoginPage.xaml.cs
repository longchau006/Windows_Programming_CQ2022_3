using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Security.Cryptography;
using Windows.Storage;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Text;
using Windows_Programming.Database;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private FirebaseServicesDAO firebaseServices;

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

        private async void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            string emailInput = EmailInputLogin_TextBox.Text;
            string passwordInput = PasswordInputLogin_TextBox.Password;



            if (emailInput == "" || passwordInput == "")
            {
                ShowDialog(emailInput == "" ? "Please enter email." : "Please enter password.");
                return;
            }

            await ReadFromDatabase(emailInput,passwordInput);
            //if (emailInput != emailDatabase || passwordInput != passwordDatabase)
            //{
            //    ShowDialog("Email or password is incorrect.");
            //    return;
            //}
            
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
                

                // Get user info
                var user = userCredential.User;
                var email = user.Info.Email;
                var uid = user.Uid;

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
            catch (Exception ex)
            {
                ShowDialog($"{ex.Message}");
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