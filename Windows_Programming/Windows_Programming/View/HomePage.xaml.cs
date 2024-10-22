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
using Windows_Programming.Model;
using Windows_Programming.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public PlansInHomeViewModel MyPlansHomeViewModel { get; set; }
        public HomePage()
        {
            this.InitializeComponent();

            Schedule_Panel.Visibility = Visibility.Collapsed;
            NoSchedule_Panel.Visibility = Visibility.Visible;

            MyPlansHomeViewModel = new PlansInHomeViewModel();
            MyPlansHomeViewModel.Init();
            if (MyPlansHomeViewModel != null)
            {
                Schedule_Panel.Visibility = Visibility.Visible;
                NoSchedule_Panel.Visibility = Visibility.Collapsed;
            }
        }

        private void HomeNagigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = args.SelectedItem as NavigationViewItem;
            if (selectedItem != null)
            {
                if (selectedItem == AccountMenuItem)
                {
                    AccountMenuFlyout.ShowAt(AccountMenuItem);
                    Home_Nagigation.SelectedItem = null;
                }
                else
                {
                    string selectedTag = selectedItem.Tag.ToString();
                    Type pageType = null;

                    switch (selectedTag)
                    {
                        case "HomePage":
                            pageType = typeof(HomePage);
                            break;
                        case "TrashCanPage":
                            pageType = typeof(TrashCanPage);
                            break;
                        case "TourPage":
                            pageType = typeof(TourPage);
                            break;
                        case "BlogPage":
                            pageType = typeof(BlogPage);
                            break;
                    }
                    if (pageType != null)
                    {
                        // Điều hướng sang trang mới
                        Frame.Navigate(pageType);
                    }
                }
            }

        }
        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            // Kiểm tra nếu có thể quay lại trang trước đó
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
        private void OnNavigationProfileClick(object sender, RoutedEventArgs e)
        {
            // Điều hướng đến trang AccountPage
            Frame.Navigate(typeof(AccountPage));
        }
        private void OnNavigationButtonClick(object sender, RoutedEventArgs e)
        {

            UpcomingTrips_Button.Style = (Style)Resources["ButtonStyle"];
            PastTrips_Button.Style = (Style)Resources["ButtonStyle"];

            Button clickedButton = sender as Button;
            clickedButton.Style = (Style)Resources["SelectedButtonStyle"];

            // Ki?m tra nút nào ?ã ???c nh?n và thay ??i n?i dung TextBlock t??ng ?ng
            if (clickedButton == UpcomingTrips_Button)
            {
                NoSchedule_TextBlock.Text = "No Upcoming Trips";
            }
            else if (clickedButton == PastTrips_Button)
            {
                NoSchedule_TextBlock.Text = "No Past Trips";
            }
        }

        private void OnNavigationFilterButtonClick(object sender, RoutedEventArgs e)
        {

            Traveler_Button.Style = (Style)Resources["FilterButtonStyle"];
            NonTraveler_Button.Style = (Style)Resources["FilterButtonStyle"];
            All_Button.Style = (Style)Resources["FilterButtonStyle"];

            Button clickedButton = sender as Button;
            clickedButton.Style = (Style)Resources["SelectedFilterButtonStyle"];

            /*while (clickedButton != null)
            {
                if (clickedButton == Traveler)
                {
                }
                else if (clickedButton == Non_Traveler)
                {
                }
                else
                {
                }
            }*/
        }

        private void OnNavigationAddButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddTripPage));
        }

        private void OnNavigationButtonNameTripClick(object sender, RoutedEventArgs e)
        {
            var selectedPlan = (sender as Button).DataContext as Plan;

            // Điều hướng đến PlanTripPage và truyền đối tượng Plan
            if (selectedPlan != null)
            {
                Frame.Navigate(typeof(PlanTripPage), selectedPlan);
            }

        }
    }
}
