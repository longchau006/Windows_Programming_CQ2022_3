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
using Windows.Gaming.Input.ForceFeedback;
using Windows.System;
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

        bool check1 = false, check2 = false, check3 = false, check4 = false, check5 = false;
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


                // Ghi đối tượng lên Firestore
                try
                {
                    await firebaseServices.UpdateWhenDeletePlanInFirestore(accountId, selectedPlan.Id, selectedPlan);

                    MyPlansInTrashCanViewModel.AddPlanInTrashCan(selectedPlan);
                    MyPlansHomeViewModel.RemovePlanInHome(selectedPlan);
                }
                catch (Exception ex)
                {
                    selectedPlan.DeletedDate = null;

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

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = Search_TextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Show all plans when search box is empty
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    plan.IsVisible = true;
                }
                return;
            }

            foreach (var plan in MyPlansHomeViewModel.PlansInHome)
            {
                // Search in name, start location, and end location
                bool matchesSearch = plan.Name.ToLower().Contains(searchText) ||
                                   plan.StartLocation.ToLower().Contains(searchText) ||
                                   plan.EndLocation.ToLower().Contains(searchText);

                plan.IsVisible = matchesSearch;
            }
        }

        private void OnNavigationCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var selectedCheckBox = sender as CheckBox;

            if (selectedCheckBox == Traveller_CheckBox)
            {
                NonTraveller_CheckBox.IsChecked = false;
                check1 = true;
                check2 = false;
            }
            else if (selectedCheckBox == NonTraveller_CheckBox)
            {
                Traveller_CheckBox.IsChecked = false;
                check2 = true;
                check1 = false;
            }
            else if (selectedCheckBox == UpcomingTrips_CheckBox)
            {
                PastTrips_CheckBox.IsChecked = false;
                CurrentTrips_CheckBox.IsChecked = false;
                check3 = true;
                check4 = false;
                check5 = false;
               
            }
            else if (selectedCheckBox == PastTrips_CheckBox)
            {
                UpcomingTrips_CheckBox.IsChecked = false;
                CurrentTrips_CheckBox.IsChecked = false;
                check4 = true;
                check3 = false;
                check5 = false;
            }
            else if (selectedCheckBox == CurrentTrips_CheckBox)
            {
                UpcomingTrips_CheckBox.IsChecked = false;
                PastTrips_CheckBox.IsChecked = false;
                check5 = true;
                check3 = false;
                check4 = false;
            }

            CheckFilter();
            
        }
        private void OnNavigationCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            var uncheckedCheckBox = sender as CheckBox;

            if (uncheckedCheckBox == Traveller_CheckBox)
            {
                check1 = false;
            }
            else if (uncheckedCheckBox == NonTraveller_CheckBox)
            {
                check2 = false;
            }
            else if (uncheckedCheckBox == UpcomingTrips_CheckBox)
            {
                check3 = false;
            }
            else if (uncheckedCheckBox == PastTrips_CheckBox)
            {
                check4 = false;
            }
            else if (uncheckedCheckBox == CurrentTrips_CheckBox)
            {
                check5 = false;
            }

            CheckFilter();  
        }

        public void CheckFilter()
        {
            DateTime nowTime = DateTime.Now;

            if (!check1 && !check2 && !check3 && !check4 && !check5)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    plan.IsVisible = true;
                }
                return;
            }
            else if (check1 && check3)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.Type && plan.StartDate > nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check1 && check4)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.Type && plan.EndDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check1 && check5)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.Type && plan.EndDate > nowTime && plan.StartDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check2 && check3)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (!plan.Type && plan.StartDate > nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check2 && check4)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (!plan.Type && plan.EndDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check2 && check5)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (!plan.Type && plan.EndDate > nowTime && plan.StartDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check1)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    plan.IsVisible = plan.Type;
                }
                return;
            }
            else if (check2)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    plan.IsVisible = !plan.Type;
                }
                return;
            }
            else if (check3)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.StartDate > nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check4)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.EndDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check5)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.EndDate > nowTime && plan.StartDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
        }
    }
}
