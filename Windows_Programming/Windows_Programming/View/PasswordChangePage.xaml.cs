using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows_Programming.ViewModel;
using Windows_Programming.Helpers;
using Windows.Storage;
using System.Windows;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PasswordChangePage : Page
    {
        private Window _mainWindow;
        public PasswordChangePage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Nhận tham chiếu từ MainWindow
            if (e.Parameter is Window mainWindow)
            {
                _mainWindow = mainWindow;
            }
        }


        private async void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OldPasswordBox.Password) || string.IsNullOrWhiteSpace(NewPasswordBox.Password) || string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password))
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Change Password",
                    Content = "Please enter all required information!",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
                return;
            }
            if (CheckInput.CheckFormatPassword(OldPasswordBox.Password) == false)
            {
                ShowDialog("Old Password Error. Password include at least 8 characters, 1 uppercase, 1 lowercase, 1 number, 1 special character: @ $ ! % * ? &");
                return;
            }
            if (CheckInput.CheckFormatPassword(NewPasswordBox.Password) == false)
            {
                ShowDialog("Password include at least 8 characters, 1 uppercase, 1 lowercase, 1 number, 1 special character: @ $ ! % * ? &");
                return;
            }
            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ShowDialog("Password and Confirm Password do not match!");
                return;
            }

            AccountViewModel accountViewModel = new AccountViewModel();
            try
            {
                await accountViewModel.UpdatePasswordAsync(OldPasswordBox.Password, NewPasswordBox.Password);
            }
            catch (Exception ex)
            {
                ContentDialog changePasswordError = new ContentDialog
                {
                    Title = "Change Password",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await changePasswordError.ShowAsync();
                return;
            }
            ContentDialog successDialog = new ContentDialog
            {
                Title = "Change Password",
                Content = "Change password successfully! You need to log in again to continue.",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await successDialog.ShowAsync();
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

        // ... other using directives

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
    }
}
