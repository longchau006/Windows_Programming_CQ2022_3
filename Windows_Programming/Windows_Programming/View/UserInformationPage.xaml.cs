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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserInformationPage : Page
    {
        public AccountViewModel accountViewModel = new AccountViewModel();
        public string fullname = "";
        public string address = "";

        public UserInformationPage()
        {
            this.InitializeComponent();
            LoadUserInformationAsync();
        }

        private async void LoadUserInformationAsync()
        {
            await accountViewModel.getInformationAsync();
            Username_TextBox.Text = accountViewModel.User.Username;
            Fullname_TextBox.Text = accountViewModel.User.Fullname;
            Email_TextBox.Text = accountViewModel.User.Email;
            Address_TextBox.Text = accountViewModel.User.Address;
        }


        private void ModifyFullnameClick(object sender, RoutedEventArgs e)
        {
            fullname = Fullname_TextBox.Text;
            SaveFullname_Button.Visibility = Visibility.Visible;
            CancelFullname_Button.Visibility = Visibility.Visible;
            ModifyFullname_Button.Visibility = Visibility.Collapsed;
            Fullname_TextBox.IsReadOnly = false;
        }


        private void ModifyAddressClick(object sender, RoutedEventArgs e)
        {
            address = Address_TextBox.Text;
            SaveAddress_Button.Visibility = Visibility.Visible;
            CancelAddress_Button.Visibility = Visibility.Visible;
            ModifyAddress_Button.Visibility = Visibility.Collapsed;
            Address_TextBox.IsReadOnly = false;
        }


        private async void SaveFullnameClick(object sender, RoutedEventArgs e)
        {
            ModifyFullname_Button.Visibility = Visibility.Visible;
            SaveFullname_Button.Visibility = Visibility.Collapsed;
            CancelFullname_Button.Visibility = Visibility.Collapsed;
            if (fullname == Fullname_TextBox.Text)
            {
                return;
            }
            await accountViewModel.UpdateFullNameAsync(Fullname_TextBox.Text);
            LoadUserInformationAsync();
        }

        private void CancelFullnameClick(object sender, RoutedEventArgs e)
        {
            Fullname_TextBox.Text = accountViewModel.User.Fullname;
            SaveFullname_Button.Visibility = Visibility.Collapsed;
            CancelFullname_Button.Visibility = Visibility.Collapsed;
            ModifyFullname_Button.Visibility = Visibility.Visible;
            Fullname_TextBox.IsReadOnly = true;
        }


        private async void SaveAddressClick(object sender, RoutedEventArgs e)
        {

            ModifyAddress_Button.Visibility = Visibility.Visible;
            SaveAddress_Button.Visibility = Visibility.Collapsed;
            CancelAddress_Button.Visibility = Visibility.Collapsed;
            if (address == Address_TextBox.Text)
            {
                return;
            }
            await accountViewModel.UpdateAddressAsync(Address_TextBox.Text);
            LoadUserInformationAsync();
        }

        private void CancelAddressClick(object sender, RoutedEventArgs e)
        {
            Address_TextBox.Text = accountViewModel.User.Address;
            SaveAddress_Button.Visibility = Visibility.Collapsed;
            CancelAddress_Button.Visibility = Visibility.Collapsed;
            ModifyAddress_Button.Visibility = Visibility.Visible;
            Address_TextBox.IsReadOnly = true;
        }
    }
}
