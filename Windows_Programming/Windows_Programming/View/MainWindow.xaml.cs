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
using Windows_Programming.ViewModel;
using Windows_Programming.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Security.Cryptography;
using Windows.Storage;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Text;
using Windows_Programming.Database;
using System.Threading.Tasks;
using Windows_Programming.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private static PlansInHomeViewModel _myPlansHomeViewModel;
        private static PlansInTrashCanViewModel _myPlansInTrashCanViewModel;
        private static Account myAccount=null;
        public MainWindow() 
        {
            this.InitializeComponent();
            //Khoi tao user after go tu dang nhap hoac dang ki
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            myAccount = new Account
            {
                Id = -1,
                Email = "Xamxaml",
                Username = "Xamxaml",
                Address = "Xamxaml",
                Fullname = "Xamxaml",
            };

            // Kiểm tra "Id" có tồn tại trong localSettings không
            if (localSettings.Values.ContainsKey("Id"))
            {
                // Chuyển giá trị của "Id" thành int
                myAccount.Id = int.TryParse(localSettings.Values["Id"]?.ToString(), out int id) ? id : 0;
            }

            // Kiểm tra và lấy các giá trị khác
            if (localSettings.Values.ContainsKey("Username"))
                myAccount.Username = localSettings.Values["Username"]?.ToString();


            if (localSettings.Values.ContainsKey("Email"))
                myAccount.Email = localSettings.Values["Email"]?.ToString();

            if (localSettings.Values.ContainsKey("Fullname"))
                myAccount.Fullname = localSettings.Values["Fullname"]?.ToString();

            if (localSettings.Values.ContainsKey("Address"))
                myAccount.Address = localSettings.Values["Address"]?.ToString();
            myAccount.PrintAccountInfo();

            _myPlansHomeViewModel = new PlansInHomeViewModel();
            _myPlansHomeViewModel.Init();

            _myPlansInTrashCanViewModel = new PlansInTrashCanViewModel();
            _myPlansInTrashCanViewModel.Init();


            //contentNavigation.Navigate(typeof(HomePage));
            Home_Nagigation.SelectedItem = Home_Nagigation.MenuItems[0];

    

        }

        private void HomeNagigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = args.SelectedItem as NavigationViewItem;
            if (selectedItem != null)
            {
                if (selectedItem == SignOut)
                {
                    SignOutUser();
                    Home_Nagigation.SelectedItem = null;
                }
                else
                {
                    string selectedTag = selectedItem.Tag.ToString();
                    Type pageType = null;
                    Window w = null;

                    switch (selectedTag)
                    {
                        case "HomePage":
                            pageType = typeof(HomePage);
                            break;
                        case "TrashCanPage":
                            pageType = typeof(TrashCanPage);
                            break;
                        case "TourPage":
                            pageType = typeof(TourListPage);
                            break;
                        case "BlogPage":
                            pageType = typeof(BlogListPage);
                            break;
                        case "AccountPage":
                            pageType = typeof(AccountPage);
                            w = this;
                            break;
                    }
                    if (pageType != null)
                    {
                        // Điều hướng sang trang mới
                        contentNavigation.Navigate(pageType, w);
                    }
                }
            }

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

            // clear all data
            myAccount = null;
            _myPlansHomeViewModel = null;
            _myPlansInTrashCanViewModel = null;

            // Navigate to LoginPage
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Activate();
            
            this.Close();
        }
        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            // Kiểm tra nếu có thể quay lại trang trước đó
            if (contentNavigation.CanGoBack)
            {
                contentNavigation.GoBack();
            }
        }

        public static PlansInHomeViewModel MyPlansHomeViewModel
        {
            get { return _myPlansHomeViewModel; }
        }

        public static PlansInTrashCanViewModel MyPlansTrashCanViewModel
        {
            get { return _myPlansInTrashCanViewModel; }
        }
        public static Account MyAccount
        {
            get
            {
                return myAccount;
            }
        }
    }
}
