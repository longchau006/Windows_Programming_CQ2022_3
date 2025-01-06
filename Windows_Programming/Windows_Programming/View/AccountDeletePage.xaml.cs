using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows_Programming.Helpers;
using Windows_Programming.ViewModel;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountDeletePage : Page
    {
        private ContentDialog loadingDialog;

        private Window _mainWindow;
        public AccountDeletePage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Receive reference from MainWindow
            if (e.Parameter is Window mainWindow)
            {
                _mainWindow = mainWindow;
            }
        }

        private async void DeleteAccountClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Delete Account",
                    Content = "Please enter your password!",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
                return;
            }
            if (CheckInput.CheckFormatPassword(PasswordBox.Password) == false)
            {
                ShowDialog("Old Password Error. Password include at least 8 characters, 1 uppercase, 1 lowercase, 1 number, 1 special character: @ $ ! % * ? &");
                return;
            }
            CreateLoadingDialog();
            AccountViewModel accountViewModel = new AccountViewModel();
            try
            {
                var loadingTask = loadingDialog.ShowAsync();
                await accountViewModel.DeleteUserAsync(MainWindow.MyAccount.Email, PasswordBox.Password);
            }
            catch (Exception ex)
            {
                loadingDialog.Hide();
                ContentDialog deletePasswordError = new ContentDialog
                {
                    Title = "Change Password",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await deletePasswordError.ShowAsync();
                return;
            }
            loadingDialog.Hide();
            ContentDialog dialog = new ContentDialog
            {
                Title = "Delete Account",
                Content = "Account deletion successful!",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();
            SignOutUser();
        }

        private void ShowDialog(string message)
        {
            NotificationPassword_TextBlock.Text = message;
            NotificationPassword_TextBlock.Opacity = 1;
            NotificationPassword_TextBlock.Visibility = Visibility.Visible;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, e) =>
            {
                NotificationPassword_TextBlock.Opacity = 0;
                NotificationPassword_TextBlock.Visibility = Visibility.Collapsed;
                timer.Stop(); // 
            };
            timer.Start(); //
        }
        private void SignOutUser()
        {
            // Clear local settings
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove("UserToken");
            localSettings.Values.Remove("Id");
            localSettings.Values.Remove("Username");
            localSettings.Values.Remove("Email");
            localSettings.Values.Remove("Fullname");
            localSettings.Values.Remove("Address");

            // Navigate to LoginPage
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Activate();
            _mainWindow.Close();

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

    }
}
