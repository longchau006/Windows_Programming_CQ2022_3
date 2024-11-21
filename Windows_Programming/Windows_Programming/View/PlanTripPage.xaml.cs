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
using Windows_Programming.Model;
using Windows_Programming.ViewModel;
using System.Diagnostics;
using Windows_Programming.Database;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlanTripPage : Page
    {
        private FirebaseServicesDAO firebaseServices;
        public Plan PlanTripViewModel { get; set; }
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        public PlansInTrashCanViewModel MyPlansInTrashCanViewModel => MainWindow.MyPlansTrashCanViewModel;

        int accountId = MainWindow.MyAccount.Id;
        public PlanTripPage()
        {
            this.InitializeComponent();
            firebaseServices = FirebaseServicesDAO.Instance;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            PlanTripViewModel = e.Parameter as Plan;

            if (PlanTripViewModel != null)
            {
                this.DataContext = PlanTripViewModel;
            }
        }
        private async void OnNavigationDeleteTripClick(object sender, RoutedEventArgs e)
        {
            if (PlanTripViewModel != null)
            {
                // Gọi phương thức xóa kế hoạch từ PlansInHomeViewModel
                PlanTripViewModel.DeletedDate = DateTime.Now;
                MyPlansInTrashCanViewModel.AddPlanInTrashCan(PlanTripViewModel);
                MyPlansHomeViewModel.RemovePlanInHome(PlanTripViewModel);

                // Ghi đối tượng lên Firestore
                try
                {
                    await firebaseServices.UpdateWhenDeletePlanInFirestore(accountId, PlanTripViewModel.Id, PlanTripViewModel);
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

                // Chuyển hướng về trang Home sau khi xóa 
                Frame.Navigate(typeof(HomePage));
            }
        }
        private void OnNavigationEditTripInfoForTripClick(object sender, RoutedEventArgs e)
        {
            if (PlanTripViewModel != null)
            {
                Frame.Navigate(typeof(EditTripPage), PlanTripViewModel);
            }
        }
        private void OnNavigationAddActivitiesForTripClick(object sender, RoutedEventArgs e)
        {
                Frame.Navigate(typeof(AddActivitiesTripPage), PlanTripViewModel);
        }

        private async void OnNavigationDeleteActivityClick(object sender, RoutedEventArgs e)
        {
            var selectedActivity = (sender as MenuFlyoutItem).CommandParameter as Model.Activity;
            if (selectedActivity != null)
            {
                MyPlansHomeViewModel.DeleteActivityForPlan(PlanTripViewModel, selectedActivity);

                // Xóa đối tượng trên Firestore
                try
                {
                    await firebaseServices.DeleteActivityInFirestore(accountId, PlanTripViewModel.Id, selectedActivity.Id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to delete activity to Firestore: {ex.Message}");

                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Failed to delete activity in home to Firestore.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    _ = errorDialog.ShowAsync();
                    return;
                }

            }
            Frame.Navigate(typeof(PlanTripPage), PlanTripViewModel);
        }
        private void OnNavigationUpdateActivityClick(object sender, RoutedEventArgs e)
        {
            var selectedActivity = (sender as MenuFlyoutItem).CommandParameter as Model.Activity;
            if (selectedActivity != null)
            { 
                var parameters = Tuple.Create(PlanTripViewModel, selectedActivity);
                Frame.Navigate(typeof(EditActivityPage), parameters);
            }
        }

    }
}
