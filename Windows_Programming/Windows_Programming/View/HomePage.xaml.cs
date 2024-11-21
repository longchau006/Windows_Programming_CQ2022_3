using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows_Programming.Database;
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
        private FirebaseServicesDAO firebaseServices;
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        public PlansInTrashCanViewModel MyPlansInTrashCanViewModel => MainWindow.MyPlansTrashCanViewModel;

        int accountId = MainWindow.MyAccount.Id;
        public HomePage()
        {
            this.InitializeComponent();
            firebaseServices = FirebaseServicesDAO.Instance;
            OnNoNoSchedulePanel();

        }
        public async void OnNoNoSchedulePanel()
        {
            Schedule_Panel.Visibility = Visibility.Visible;
            NoSchedule_Panel.Visibility = Visibility.Collapsed;
            var numPlan = await firebaseServices.GetNumAllPlanInHome(accountId);
            if (numPlan == 0)
            {
                NoSchedule_Panel.Visibility = Visibility.Visible;
                Schedule_Panel.Visibility = Visibility.Collapsed;
            }
        }

        private void OnNavigationButtonClick(object sender, RoutedEventArgs e)
        {

            UpcomingTrips_Button.Style = (Style)Resources["FilterButtonStyle"];
            PastTrips_Button.Style = (Style)Resources["FilterButtonStyle"];

            Button clickedButton = sender as Button;
            clickedButton.Style = (Style)Resources["SelectedFilterButtonStyle"];

            /*if (clickedButton == UpcomingTrips_Button)
            {
               
            }
            else if (clickedButton == PastTrips_Button)
            {
                
            }*/
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
        private void OnNavigationEditTripInfoForTripClick(object sender, RoutedEventArgs e)
        {
            var selectedPlan = (sender as Button).DataContext as Plan;
            if (selectedPlan != null)
            {
                Frame.Navigate(typeof(EditTripPage), selectedPlan);
            }
        }
        private async void OnNavigationDeleteTripClick(object sender, RoutedEventArgs e)
        {
            var selectedPlan = (sender as MenuFlyoutItem).CommandParameter as Plan;
            if (selectedPlan != null)
            {
                // Gọi phương thức xóa kế hoạch từ PlansInHomeViewModel
                selectedPlan.DeletedDate = DateTime.Now;
                MyPlansInTrashCanViewModel.AddPlanInTrashCan(selectedPlan);
                MyPlansHomeViewModel.RemovePlanInHome(selectedPlan);

                // Ghi đối tượng lên Firestore
                try
                {
                    await firebaseServices.UpdateWhenDeletePlanInFirestore(accountId, selectedPlan.Id, selectedPlan);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to save to Firestore: {ex.Message}");

                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Failed to delete the trip in home to Firestore.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    _ = errorDialog.ShowAsync();
                    return;
                }
            }
        }
    }
}
