using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountPage : Page
    {
        public List<NavLink> NavLinks { get; set; }
        private Window _mainWindow;

        public AccountPage()
        {
            this.InitializeComponent();
            NavLinks = new List<NavLink>
            {
                new NavLink { Name = "USER INFORMATION", Tag = "UserInformationPage", Symbol = Symbol.OtherUser },
                new NavLink { Name = "CHANGE PASSWORD", Tag = "ChangePasswordPage", Symbol = Symbol.Admin  },
                new NavLink { Name = "DELETE ACCOUNT", Tag = "DeleteAccountPage", Symbol = Symbol.Delete }
            };
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


        void Account_SelectionChanged(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem as NavLink;
            if (clickedItem != null)
            {
                switch (clickedItem.Tag)
                {
                    case "UserInformationPage":
                        splitviewContent.Text = "User Information";
                        AccountFrame.Navigate(typeof(UserInformationPage));
                        break;
                    case "ChangePasswordPage":
                        splitviewContent.Text = "Change Password";
                        AccountFrame.Navigate(typeof(PasswordChangePage), _mainWindow);
                        break;
                    case "DeleteAccountPage":
                        splitviewContent.Text = "Delete Account";
                        AccountFrame.Navigate(typeof(AccountDeletePage), _mainWindow);
                        break;
                }
            }
        }
    }

    public class NavLink
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public Symbol Symbol { get; set; }
    }
}
