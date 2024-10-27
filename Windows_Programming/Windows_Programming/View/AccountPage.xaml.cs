using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountPage : Page
    {
        public AccountPage()
        {
            this.InitializeComponent();
        }

        private void Account_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            switch (args.SelectedItemContainer.Tag)
            {
                case "UserInformationPage":
                    AccountFrame.Navigate(typeof(UserInformationPage));
                    break;
                case "ChangePasswordPage":
                    AccountFrame.Navigate(typeof(PasswordChangePage));
                    break;
                case "DeleteAccountPage":
                    AccountFrame.Navigate(typeof(AccountDeletePage));
                    break;
            }
            
        }

        private void nvAccount_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {

        }

        private void Account_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {

        }
    }
}
