﻿using System;
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

            AccountViewModel accountViewModel = new AccountViewModel();
            try
            {
                await accountViewModel.DeleteUserAsync(MainWindow.MyAccount.Email, PasswordBox.Password);
            }
            catch (Exception ex)
            {
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
    }
}
